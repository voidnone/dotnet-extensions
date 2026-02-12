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

        return services;
    }

    private static void AddFromType(IServiceCollection services, Type type)
    {
        var attributes = type.GetCustomAttributes<LifetimeAttribute>();

        foreach (var attribute in attributes)
        {
            if (attribute == default) continue;
            AddFromAttribute(services, type, attribute);
        }
    }

    private static void AddFromAttribute(IServiceCollection services, Type type, LifetimeAttribute attribute)
    {
        var typeServices = new Stack<Type>(attribute.Services);

        if (!typeServices.TryPop(out var firstService))
        {
            firstService = type;
        }

#if NET8_0_OR_GREATER
        services.Add(new ServiceDescriptor(firstService, attribute.Key, type, attribute.ServiceLifetime));
#else
                services.Add(new ServiceDescriptor(firstService, type, attribute.ServiceLifetime));
#endif

        foreach (var typeService in typeServices)
        {
#if NET8_0_OR_GREATER
            services.Add(new ServiceDescriptor(typeService, attribute.Key, (s, key) => s.GetRequiredKeyedService(firstService, key), attribute.ServiceLifetime));
#else
                 services.Add(new ServiceDescriptor(typeService, s => s.GetRequiredService(firstService), attribute.ServiceLifetime));
#endif

        }
    }
}