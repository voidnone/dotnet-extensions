using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    private static readonly ConcurrentDictionary<Type, IEnumerable<Type>> serviceTypes = [];
    private static readonly ConcurrentDictionary<Type, IEnumerable<object>> serviceInstances = [];

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

    public static T[] GetAllServices<T>(this IServiceProvider serviceProvider) => [.. GetAllServices(serviceProvider, typeof(T)).Select(s => (T)s)];

    public static object[] GetAllServices(this IServiceProvider serviceProvider, Type serviceType)
    {
        IEnumerable<object> GetAllServices(Type type)
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

            foreach (var item in serviceProvider.GetServices(type))
            {
                if (item != null) yield return item;
            }

            foreach (var key in keys)
            {
                foreach (var item in serviceProvider.GetKeyedServices(type, key))
                {
                    if (item != null) yield return item;
                }
            }
        }

        return [.. serviceInstances.GetOrAdd(serviceType, GetAllServices)];
    }
#endif
}