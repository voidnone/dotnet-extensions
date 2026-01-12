using System.Collections.Concurrent;

namespace VoidNone.Logging.Core;

public abstract class QueueLogWriter : ILogWriter
{
    private readonly static ConcurrentQueue<Log> queue = new();
    private static uint writing = 0;
    private Exception? exception;

    public void WriteLog(Log log)
    {
        if (exception != null) throw exception;
        queue.Enqueue(log);
        _ = WriteLogAsync();
    }

    private async Task WriteLogAsync()
    {
        if (Interlocked.Exchange(ref writing, 1) == 0)
        {
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
            if (!queue.IsEmpty) _ = WriteLogAsync();
        }
    }

    protected abstract Task WriteLogAsync(Log log);
}