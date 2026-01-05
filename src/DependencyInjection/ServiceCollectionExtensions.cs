using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

        var types = assemblies.SelectMany(s => s.GetTypes()).Distinct().ToArray();

        foreach (var type in types)
        {
            if (type.IsAbstract || type.IsInterface) continue;
            var attributes = type.GetCustomAttributes<LifetimeAttribute>();

            foreach (var attribute in attributes)
            {
                if (attribute == default) continue;
                var typeServices = attribute.Services.Length == 0 ? new[] { type } : attribute.Services;

                foreach (var typeService in typeServices)
                {
#if NET8_0_OR_GREATER
                    services.Add(new ServiceDescriptor(typeService, attribute.Key, type, attribute.ServiceLifetime));
#else
                services.Add(new ServiceDescriptor(typeService, type, attribute.ServiceLifetime));
#endif

                }
            }
        }

        return services;
    }
}