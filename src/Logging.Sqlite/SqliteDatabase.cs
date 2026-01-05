using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Huanent.Logging.Core;
using Microsoft.Data.Sqlite;

namespace Huanent.Logging.Sqlite;

public class SqliteDatabase : IDisposable
{
    private readonly string path;
    private readonly SqliteConnection connection;
    private readonly InsertCommand insertCommand;
    private long ticks;

    public string Path => path;

    public SqliteDatabase(string path)
    {
        this.path = path;

        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Cache = SqliteCacheMode.Shared,
        };

        connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
        Initialize();
        insertCommand = new InsertCommand(connection);
    }

    private void Initialize()
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
        CREATE TABLE IF NOT EXISTS `Log` (
            Id INTEGER PRIMARY KEY,
            Level INTEGER NOT NULL,
            Name TEXT NOT NULL,
            Message TEXT NOT NULL,
            EventId INTEGER NOT NULL,
            Exception TEXT
        );

        CREATE INDEX IF NOT EXISTS Log_Level_INDEX
        ON `Log`(Level);

        CREATE INDEX IF NOT EXISTS Log_Name_INDEX
        ON `Log`(Name);
        """;

        command.ExecuteNonQuery();
    }

    public async Task AddLogAsync(IEnumerable<Log> logs)
    {
        var transaction = await connection.BeginTransactionAsync();

        foreach (var log in logs)
        {
            if (ticks < log.CreationTime.Ticks)
            {
                ticks = log.CreationTime.Ticks;
            }
            else
            {
                ticks++;
            }

            await insertCommand.ExecuteAsync(
                ticks,
                (int)log.Level,
                 log.Name,
                 log.Message,
                 log.EventId.Id,
                 log.Exception?.ToString()
            );
        }

        await transaction.CommitAsync();
    }

    public void Dispose()
    {
        insertCommand.Dispose();
        SqliteConnection.ClearPool(connection);
    }
}