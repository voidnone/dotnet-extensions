using Huanent.Logging.Core;
using Huanent.Logging.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.Logging;

public static class SqliteLoggerFactoryExtensions
{
    public static ILoggingBuilder AddSqlite(this ILoggingBuilder builder, Action<SqliteLoggerOptions> options = null)
    {
        builder.Services.AddOptions<SqliteLoggerOptions>();
        if (options != default) builder.Services.Configure(options);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogWriter, SqliteLogWriter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, SqliteLoggerProvider>());
        return builder;
    }
}

