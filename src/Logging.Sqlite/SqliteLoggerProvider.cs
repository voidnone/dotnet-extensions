using Microsoft.Extensions.Logging;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.Sqlite;

[ProviderAlias("Sqlite")]
public class SqliteLoggerProvider(ILogWriter loggerWriter) : LoggerProvider(loggerWriter)
{
}

