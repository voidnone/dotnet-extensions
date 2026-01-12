using Microsoft.Extensions.Options;
using System.Text;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.File;

public class FileLogWriter : QueueLogWriter
{
    private readonly FileLoggerOptions options;
    private readonly string prefixPath;

    public FileLogWriter(IOptions<FileLoggerOptions> options)
    {
        this.options = options.Value;
        prefixPath = Path.Combine(AppContext.BaseDirectory, this.options.Path);
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

    protected override async Task WriteLogAsync(Log log)
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
}

