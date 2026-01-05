using Huanent.Logging.Core;
using Microsoft.Extensions.Logging;

namespace Huanent.Logging.Sqlite;

[ProviderAlias("Sqlite")]
public class SqliteLoggerProvider(ILogWriter loggerWriter) : LoggerProvider(loggerWriter)
{
}

