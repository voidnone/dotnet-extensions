using Huanent.Logging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging;

public static class LoggerFactoryExtensions
{
    public static ILoggingBuilder AddImplementation<T>(this ILoggingBuilder builder) where T : class, ILogWriter
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogWriter, T>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());
        return builder;
    }
}
