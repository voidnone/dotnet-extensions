using System.Collections.Concurrent;

namespace VoidNone.Logging.Core;

public abstract class QueueLogWriter : ILogWriter
{
    private readonly static ConcurrentQueue<Log> queue = new();
    private static long writing = 0;
    private Exception? exception;

    protected virtual Action? OnQueueLogWriting { get; }

    public void WriteLog(Log log)
    {
        if (exception != null) throw exception;
        queue.Enqueue(log);
        if (Interlocked.Read(ref writing) == 1) return;
        Task.Run(WriteQueueLogAsync);
    }

    private async ValueTask WriteQueueLogAsync()
    {
        if (Interlocked.Exchange(ref writing, 1) == 0)
        {
            if (OnQueueLogWriting != null)
            {
                _ = Task.Run(OnQueueLogWriting).ConfigureAwait(false);
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

            Interlocked.Exchange(ref writing, 0);
            if (!queue.IsEmpty) _ = Task.Run(WriteQueueLogAsync);
        }
    }

    protected abstract Task WriteLogAsync(Log log);
}