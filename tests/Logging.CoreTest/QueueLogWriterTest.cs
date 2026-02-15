using VoidNone.Logging.Core;

namespace Logging.CoreTest;

[TestClass]
public class QueueLogWriterTest
{
    class CustomQueueLogWriter : QueueLogWriter
    {
        public List<Log> Logs { get; set; } = [];
        protected override Task WriteLogAsync(Log log)
        {
            Logs.Add(log);
            return Task.CompletedTask;
        }
    }

    [TestMethod]
    public Task WriteLogAsync()
    {
        var customQueueLogWriter = new CustomQueueLogWriter();

        Parallel.ForEach(Enumerable.Range(0, 1000), index =>
        {
            customQueueLogWriter.WriteLog(new Log(Microsoft.Extensions.Logging.LogLevel.Error, string.Empty, new Microsoft.Extensions.Logging.EventId(index), string.Empty, null));
        });

        Assert.HasCount(1000, customQueueLogWriter.Logs);

        return Task.CompletedTask;
    }
}
