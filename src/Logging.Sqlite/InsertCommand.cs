using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Huanent.Logging.Sqlite;

public class InsertCommand : IDisposable
{
    private readonly SqliteCommand command;
    private readonly SqliteParameter idParameter;
    private readonly SqliteParameter levelParameter;
    private readonly SqliteParameter nameParameter;
    private readonly SqliteParameter messageParameter;
    private readonly SqliteParameter eventIdParameter;
    private readonly SqliteParameter exceptionParameter;

    private static readonly string text = """
    INSERT INTO `Log` (
            Id,
            Level,
            Name,
            Message,
            EventId,
            Exception
        ) VALUES (
            $Id,
            $Level,
            $Name,
            $Message,
            $EventId,
            $Exception
        )
    """;

    public InsertCommand(SqliteConnection connection)
    {
        command = connection.CreateCommand();
        command.CommandText = text;
        idParameter = command.CreateParameter();
        idParameter.DbType = System.Data.DbType.Int64;
        idParameter.ParameterName = "$Id";
        levelParameter = command.CreateParameter();
        levelParameter.ParameterName = "$Level";
        levelParameter.DbType = System.Data.DbType.Int64;
        nameParameter = command.CreateParameter();
        nameParameter.ParameterName = "$Name";
        messageParameter = command.CreateParameter();
        messageParameter.ParameterName = "$Message";
        eventIdParameter = command.CreateParameter();
        eventIdParameter.DbType = System.Data.DbType.Int64;
        eventIdParameter.ParameterName = "$EventId";
        exceptionParameter = command.CreateParameter();
        exceptionParameter.ParameterName = "$Exception";
    }

    public async Task ExecuteAsync(long id, int level, string name, string message, int eventId, string? exception)
    {
        idParameter.Value = id;
        levelParameter.Value = level;
        nameParameter.Value = name;
        messageParameter.Value = message;
        eventIdParameter.Value = eventId;
        exceptionParameter.Value = exception;
        await command.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        command.Dispose();
    }
}