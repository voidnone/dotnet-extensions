using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Automatically register services from the provided assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        var types = assemblies.SelectMany(s => s.GetTypes()).Distinct().ToArray();

        foreach (var type in types)
        {
            if (type == null || type.IsAbstract || type.IsInterface) continue;
            AddFromType(services, type);
        }

        if (services.All(a => a.ServiceType != typeof(IServiceCollection)))
        {
            services.AddSingleton(services);
        }

        return services;
    }

    private static void AddFromType(IServiceCollection services, Type type)
    {
        var attributes = type.GetCustomAttributes<LifetimeAttribute>();
        if (attributes == null) return;

        var grouped = attributes.Where(w => w != default).GroupBy(g => new { g.ServiceLifetime, g.Key });

        foreach (var group in grouped)
        {
            var serviceTypes = group.SelectMany(s => s.Services).Distinct().ToArray();

            serviceTypes.Sort((left, right) =>
            {
                if (left.IsAssignableFrom(right)) return 1;
                if (right.IsAssignableFrom(left)) return -1;
                return -1;
            });

            AddServices(services, type, group.Key.ServiceLifetime, group.Key.Key, serviceTypes);
        }
    }

    private static void AddServices(IServiceCollection services, Type implementationType, ServiceLifetime lifetime, object? key, Type[] serviceTypes)
    {
        if (serviceTypes.Length == 0)
        {
#if NET8_0_OR_GREATER
            services.Add(new ServiceDescriptor(implementationType, key, implementationType, lifetime));
#else
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
#endif
        }
        else
        {
            foreach (var typeService in serviceTypes)
            {
#if NET8_0_OR_GREATER
            services.Add(new ServiceDescriptor(typeService, key, implementationType, lifetime));
#else
                services.Add(new ServiceDescriptor(typeService, s => implementationType, lifetime));
#endif
            }
        }
    }
}