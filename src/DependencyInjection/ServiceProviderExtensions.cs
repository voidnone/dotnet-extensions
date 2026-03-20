using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    private static readonly ConcurrentDictionary<Type, IEnumerable<Type>> serviceTypes = [];
    private static readonly ConcurrentDictionary<Type, object[]> serviceKeys = [];

    public static Type[] GetAllServiceTypes<T>(this IServiceProvider serviceProvider) => GetAllServiceTypes(serviceProvider, typeof(T));

    public static Type[] GetAllServiceTypes(this IServiceProvider serviceProvider, Type serviceType)
    {
        IEnumerable<Type> GetAllServiceTypes(Type type)
        {
            var serviceCollection = serviceProvider.GetRequiredService<IServiceCollection>();

            foreach (var item in serviceCollection)
            {
                if (item.ServiceType != serviceType) continue;
#if NET8_0_OR_GREATER
                if (item.IsKeyedService && item.KeyedImplementationType != null)
                {
                    yield return item.KeyedImplementationType;
                }
                else
#endif
                    if (item.ImplementationType != null)
                    {
                        yield return item.ImplementationType;
                    }
            }
        }

        return [.. serviceTypes.GetOrAdd(serviceType, GetAllServiceTypes)];
    }

#if NET8_0_OR_GREATER

    public static IEnumerable<T> GetAllServices<T>(this IServiceProvider serviceProvider) => GetAllServices(serviceProvider, typeof(T)).Select(s => (T)s);

    public static IEnumerable<object> GetAllServices(this IServiceProvider serviceProvider, Type serviceType)
    {
        var keys = serviceKeys.GetOrAdd(serviceType, type =>
        {
            var serviceCollection = serviceProvider.GetRequiredService<IServiceCollection>();
            var keys = new HashSet<object>();

            foreach (var item in serviceCollection)
            {
                if (item.ServiceType != type) continue;
                if (item.IsKeyedService && item.ServiceKey != null)
                {
                    keys.Add(item.ServiceKey);
                }
            }
            return [.. keys];
        });

        foreach (var item in serviceProvider.GetServices(serviceType))
        {
            if (item != null) yield return item;
        }

        foreach (var key in keys)
        {
            foreach (var item in serviceProvider.GetKeyedServices(serviceType, key))
            {
                if (item != null) yield return item;
            }
        }
    }
#endif
}