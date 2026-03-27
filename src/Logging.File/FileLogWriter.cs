using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.File;

public class FileLogWriter : QueueLogWriter
{
    private readonly FileLoggerOptions options;
    private readonly string directory;
    private DateTime nextCleanTime = DateTime.UtcNow;

    protected override Action? OnQueueLogWriting => ClearLogFile;

    public FileLogWriter(IOptions<FileLoggerOptions> options)
    {
        this.options = options.Value;
        directory = Path.Combine(AppContext.BaseDirectory, this.options.Path);
    }

    private void ClearLogFile()
    {
        if (DateTime.UtcNow < nextCleanTime) return;
        if (!Directory.Exists(directory)) return;

        nextCleanTime = DateTime.UtcNow.AddDays(1);
        var files = Directory.GetFiles(directory, "*.log", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var lastWriteTime = System.IO.File.GetLastWriteTimeUtc(file);
            if (lastWriteTime.AddDays(options.RetentionDays) > DateTime.UtcNow) continue;
            System.IO.File.Delete(file);
        }
    }

    protected override async Task WriteLogAsync(Log log)
    {
        var logBuilder = new StringBuilder();
        var splitter = $"[{log.Level}] [{log.Name}] [{log.EventId}] [{log.CreationTime:yyyy/MM/dd HH:mm:ss zzz}]";
        logBuilder.AppendLine(splitter);
        logBuilder.AppendLine(log.Message);
        if (log.Exception != default) logBuilder.AppendLine(log.Exception.ToString());
        logBuilder.AppendLine();
        var path = GetFilePath(log.Level, log.CreationTime);
        await System.IO.File.AppendAllTextAsync(path, logBuilder.ToString());
    }

    private string GetFilePath(LogLevel logLevel, DateTimeOffset creationTime)
    {
        var fileName = $"{creationTime.ToString(options.DateFormat)}.log";
        var path = Path.Combine(this.directory, logLevel.ToString(), fileName);
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        return path;
    }
}
