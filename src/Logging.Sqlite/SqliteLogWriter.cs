using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.Sqlite;

public class SqliteLogWriter : ILogWriter
{
    private readonly SqliteLoggerOptions options;
    private readonly string prefixPath;
    private readonly static ConcurrentQueue<Log> queue = new();
    private static uint writing = 0;
    private static SqliteDatabase database;

    public SqliteLogWriter(IOptions<SqliteLoggerOptions> options)
    {
        this.options = options.Value;
        prefixPath = Path.Combine(AppContext.BaseDirectory, this.options.Path);
    }

    public void WriteLog(Log log)
    {
        queue.Enqueue(log);

        if (Interlocked.Exchange(ref writing, 1) == 0)
        {
            _ = WriteLogAsync();
        }
    }

    private async Task WriteLogAsync()
    {
        var list = new List<Log>();

        while (queue.TryDequeue(out var log))
        {
            list.Add(log);
            if (list.Count > 10)
            {
                await database.AddLogAsync(list);
                list.Clear();
            }
        }

        if (list.Count > 0)
        {
            await database.AddLogAsync(list);
        }

        Interlocked.Exchange(ref writing, 0);
    }

    private SqliteDatabase GetDatabase(DateTimeOffset creationTime)
    {
        var fileName = $"{creationTime.ToString(options.DateFormat)}.db";
        var path = Path.Combine(prefixPath, fileName);
        if (database?.Path == path) return database;
        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        return new SqliteDatabase(path);
    }
}

