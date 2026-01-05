using System;
using Microsoft.Extensions.Logging;

namespace Huanent.Logging.Core;

public class Log(LogLevel level, string name, EventId eventId, string message, Exception? exception)
{
    public DateTimeOffset CreationTime { get; init; } = DateTimeOffset.UtcNow;
    public LogLevel Level { get; init; } = level;
    public string Name { get; init; } = name;
    public EventId EventId { get; init; } = eventId;
    public string Message { get; init; } = message;
    public Exception? Exception { get; init; } = exception;
}