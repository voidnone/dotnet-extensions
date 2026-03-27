using VoidNone.Logging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace VoidNone.Logging.CoreTest;

[TestClass]
public class QueueLogWriterTest
{
    private class CustomQueueLogWriter : QueueLogWriter
    {
        public ConcurrentQueue<Log> Logs { get; } = new();

        protected override Task WriteLogAsync(Log log)
        {
            Logs.Enqueue(log);
            return Task.CompletedTask;
        }
    }

    [TestMethod]
    public async Task WriteLogAsync()
    {
        var customQueueLogWriter = new CustomQueueLogWriter();

        Parallel.ForEach(Enumerable.Range(0, 1000), index =>
        {
            customQueueLogWriter.WriteLog(CreateLog(string.Empty, index.ToString(), index));
        });

        await WaitUntilAsync(() => customQueueLogWriter.Logs.Count == 1000);
        Assert.HasCount(1000, customQueueLogWriter.Logs);
    }

    [TestMethod]
    public async Task WriteLogAsync_ShouldKeepQueuesIsolatedBetweenInstances()
    {
        var firstQueueLogWriter = new CustomQueueLogWriter();
        var secondQueueLogWriter = new CustomQueueLogWriter();

        Parallel.ForEach(Enumerable.Range(0, 500), index =>
        {
            firstQueueLogWriter.WriteLog(CreateLog("first", index.ToString(), index));
            secondQueueLogWriter.WriteLog(CreateLog("second", index.ToString(), index));
        });

        await WaitUntilAsync(() => firstQueueLogWriter.Logs.Count == 500 && secondQueueLogWriter.Logs.Count == 500);

        Assert.IsTrue(firstQueueLogWriter.Logs.All(log => log.Name == "first"));
        Assert.IsTrue(secondQueueLogWriter.Logs.All(log => log.Name == "second"));
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (condition()) return;
            await Task.Delay(10);
        }

        Assert.Fail("Condition was not met within the expected time.");
    }

    private class FirstLogWriter : ILogWriter
    {
        public ConcurrentQueue<Log> Logs { get; } = new();

        public void WriteLog(Log log)
        {
            Logs.Enqueue(log);
        }
    }

    private class SecondLogWriter : ILogWriter
    {
        public ConcurrentQueue<Log> Logs { get; } = new();

        public void WriteLog(Log log)
        {
            Logs.Enqueue(log);
        }
    }

    [TestMethod]
    public void AddImplementation_ShouldBindEachProviderToItsOwnWriter()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddImplementation<FirstLogWriter>();
            builder.AddImplementation<SecondLogWriter>();
        });

        using var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var firstLogWriter = serviceProvider.GetRequiredService<FirstLogWriter>();
        var secondLogWriter = serviceProvider.GetRequiredService<SecondLogWriter>();
        var logger = loggerFactory.CreateLogger<QueueLogWriterTest>();

        logger.LogError("test message");

        Assert.HasCount(1, firstLogWriter.Logs);
        Assert.HasCount(1, secondLogWriter.Logs);
        Assert.AreEqual("test message", firstLogWriter.Logs.Single().Message);
        Assert.AreEqual("test message", secondLogWriter.Logs.Single().Message);
    }

    private class BlockingQueueLogWriter : QueueLogWriter
    {
        private readonly TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource allowWrite = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ConcurrentQueue<Log> Logs { get; } = new();

        public Task Started => started.Task;

        public void ReleaseWrites()
        {
            allowWrite.TrySetResult();
        }

        protected override async Task WriteLogAsync(Log log)
        {
            started.TrySetResult();
            await allowWrite.Task;
            Logs.Enqueue(log);
        }
    }

    [TestMethod]
    public async Task Dispose_ShouldFlushQueuedLogsBeforeReturning()
    {
        var queueLogWriter = new BlockingQueueLogWriter();

        queueLogWriter.WriteLog(new Log(LogLevel.Error, "first", new EventId(1), "first", null));
        await queueLogWriter.Started;
        queueLogWriter.WriteLog(new Log(LogLevel.Error, "second", new EventId(2), "second", null));

        var disposeTask = Task.Run(queueLogWriter.Dispose);

        await Task.Delay(50);
        Assert.IsFalse(disposeTask.IsCompleted);

        queueLogWriter.ReleaseWrites();
        await disposeTask;

        Assert.HasCount(2, queueLogWriter.Logs);
        CollectionAssert.AreEquivalent(new[] { "first", "second" }, queueLogWriter.Logs.Select(log => log.Message).ToArray());
    }

    [TestMethod]
    public void WriteLog_AfterDispose_ShouldThrowObjectDisposedException()
    {
        var queueLogWriter = new CustomQueueLogWriter();
        queueLogWriter.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            queueLogWriter.WriteLog(CreateLog("disposed", "message", 1))
        );
    }

    [TestMethod]
    public void Dispose_ImmediatelyAfterWrite_ShouldFlushQueuedLogs()
    {
        var queueLogWriter = new CustomQueueLogWriter();

        queueLogWriter.WriteLog(CreateLog("flush", "message", 1));
        queueLogWriter.Dispose();

        Assert.HasCount(1, queueLogWriter.Logs);
    }

    private static Log CreateLog(string name, string message, int eventId, Exception? exception = null)
    {
        return new Log(LogLevel.Error, name, new EventId(eventId), message, exception);
    }
}
