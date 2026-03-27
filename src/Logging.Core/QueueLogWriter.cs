using System.Collections.Concurrent;

namespace VoidNone.Logging.Core;

public abstract class QueueLogWriter : ILogWriter, IDisposable
{
    private readonly object syncRoot = new();
    private readonly ConcurrentQueue<Log> queue = new();
    private long writing = 0;
    private Exception? exception;
    private bool disposed;
    private Task processingTask = Task.CompletedTask;

    protected virtual Action? OnQueueLogWriting { get; }

    public void WriteLog(Log log)
    {
        ArgumentNullException.ThrowIfNull(log);

        lock (syncRoot)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (exception != null) throw exception;

            queue.Enqueue(log);
            if (Interlocked.Read(ref writing) == 1) return;

            processingTask = Task.Run(WriteQueueLogAsync);
        }
    }

    public void Dispose()
    {
        Task processingTask;

        lock (syncRoot)
        {
            if (disposed) return;
            disposed = true;

            if (Interlocked.Read(ref writing) == 0 && !queue.IsEmpty && this.processingTask.IsCompleted)
            {
                this.processingTask = Task.Run(WriteQueueLogAsync);
            }

            processingTask = this.processingTask;
        }

        processingTask.GetAwaiter().GetResult();
        GC.SuppressFinalize(this);

        if (exception != null) throw exception;
    }

    private async Task WriteQueueLogAsync()
    {
        if (Interlocked.Exchange(ref writing, 1) != 0) return;

        try
        {
            if (OnQueueLogWriting != null)
            {
                try
                {
                    OnQueueLogWriting();
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return;
                }
            }

            while (queue.TryDequeue(out var log))
            {
                try
                {
                    await WriteLogAsync(log);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
        }
        finally
        {
            Interlocked.Exchange(ref writing, 0);

            lock (syncRoot)
            {
                if (!disposed && !queue.IsEmpty)
                {
                    processingTask = Task.Run(WriteQueueLogAsync);
                }
            }
        }
    }

    protected abstract Task WriteLogAsync(Log log);
}
