using Microsoft.Extensions.Logging;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.File;

[ProviderAlias("File")]
public class FileLoggerProvider(ILogWriter loggerWriter) : LoggerProvider(loggerWriter)
{
}

