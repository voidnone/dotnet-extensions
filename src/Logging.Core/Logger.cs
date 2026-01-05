using Microsoft.Extensions.Logging;
using System;

namespace Huanent.Logging.Core;

public class Logger(string name, Func<string, LogLevel, bool>? filter, ILogWriter loggerWriter) : ILogger
{
    private readonly string name = string.IsNullOrWhiteSpace(name) ? nameof(Logger) : name;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) => filter?.Invoke(name, logLevel) ?? true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel)) return;
        ArgumentNullException.ThrowIfNull(formatter);
        string message = formatter(state, exception);
        if (string.IsNullOrEmpty(message)) return;
        var log = new Log(logLevel, name, eventId, message, exception);
        loggerWriter.WriteLog(log);
    }
}

