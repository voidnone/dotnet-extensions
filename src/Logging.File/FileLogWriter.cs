using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.File;

public class FileLogWriter : ILogWriter
{
    private readonly FileLoggerOptions options;
    private readonly string prefixPath;
    private readonly static ConcurrentQueue<Log> queue = new();
    private static uint writing = 0;
    private Exception? exception;

    public FileLogWriter(IOptions<FileLoggerOptions> options)
    {
        this.options = options.Value;
        prefixPath = Path.Combine(AppContext.BaseDirectory, this.options.Path);
    }

    public void WriteLog(Log log)
    {
        if (exception != null) throw exception;
        
        queue.Enqueue(log);

        if (Interlocked.Exchange(ref writing, 1) == 0)
        {
            _ = WriteLogAsync();
        }
    }

    private async Task WriteLogAsync()
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
    }

    private async Task WriteLogAsync(Log log)
    {
        var logBuilder = new StringBuilder();
        var splitter = $"[{log.Level}] [{log.Name}] [{log.EventId}] [{DateTimeOffset.UtcNow}]";
        logBuilder.AppendLine(splitter);
        logBuilder.AppendLine(log.Message);
        if (log.Exception != default) logBuilder.AppendLine(log.Exception.ToString());
        logBuilder.AppendLine();
        var path = GetFilePath(log.CreationTime);
        await System.IO.File.AppendAllTextAsync(path, logBuilder.ToString());
    }

    private string GetFilePath(DateTimeOffset creationTime)
    {
        var fileName = $"{creationTime.ToString(options.DateFormat)}.txt";
        var path = Path.Combine(prefixPath, fileName);
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        return path;
    }
}

