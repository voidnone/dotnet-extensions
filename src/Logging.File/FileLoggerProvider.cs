using Microsoft.Extensions.Logging;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.File;

[ProviderAlias("File")]
public class FileLoggerProvider(FileLogWriter loggerWriter) : LoggerProvider<FileLogWriter>(loggerWriter)
{
}
