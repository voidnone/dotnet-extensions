using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VoidNone.Logging.Core;
using VoidNone.Logging.File;

namespace Microsoft.Extensions.Logging;

public static class FileLoggerFactoryExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions>? options = null)
    {
        builder.Services.AddOptions<FileLoggerOptions>();
        if (options != default) builder.Services.Configure(options);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogWriter, FileLogWriter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        return builder;
    }
}

