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
            AddServices(services, type, group.Key.ServiceLifetime, group.Key.Key, serviceTypes);
        }
    }

    private static void AddServices(IServiceCollection services, Type type, ServiceLifetime lifetime, object? key, Type[] types)
    {
        var typeServices = new Stack<Type>(SortTypeByAssignable(types));

        if (!typeServices.TryPop(out var firstService))
        {
            firstService = type;
        }

#if NET8_0_OR_GREATER
        services.Add(new ServiceDescriptor(firstService, key, type, lifetime));
#else
        services.Add(new ServiceDescriptor(firstService, type, lifetime));
#endif

        foreach (var typeService in typeServices)
        {
#if NET8_0_OR_GREATER
            services.Add(new ServiceDescriptor(typeService, key, (s, k) => s.GetRequiredKeyedService(firstService, k), lifetime));
#else
            services.Add(new ServiceDescriptor(typeService, s => s.GetRequiredService(firstService), lifetime));
#endif

        }
    }

    internal static IEnumerable<Type> SortTypeByAssignable(Type[] types)
    {
        if (types.Length < 2) return types;
        var result = new List<Type>();

        foreach (var type in types)
        {
            var lastAssignableFrom = result.LastOrDefault(t => t.IsAssignableFrom(type));
            if (lastAssignableFrom == null)
            {
                result.Add(type);
            }
            else
            {
                result.Insert(result.IndexOf(lastAssignableFrom), type);
            }
        }

        return result;
    }
}