using Microsoft.Extensions.Logging;

namespace VoidNone.Logging.Core
{
    [ProviderAlias("Implementation")]
    public class LoggerProvider(ILogWriter loggerWriter) : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool>? filter;
        private readonly ILogWriter loggerWriter = loggerWriter;

        public LoggerProvider(Func<string, LogLevel, bool>? filter, ILogWriter loggerWriter)
            : this(loggerWriter)
        {
            this.filter = filter;
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(name, filter, loggerWriter);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (loggerWriter is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
