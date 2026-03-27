using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VoidNone.Logging.Core;

namespace Microsoft.Extensions.Logging;

public static class LoggerFactoryExtensions
{
    public static ILoggingBuilder AddImplementation<T>(this ILoggingBuilder builder) where T : class, ILogWriter
    {
        builder.Services.TryAddSingleton<T>();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider<T>>());
        return builder;
    }
}
