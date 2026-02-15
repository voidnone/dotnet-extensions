#if NET8_0_OR_GREATER
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    private static readonly ConcurrentDictionary<Type, IEnumerable<Type>> serviceTypes = [];
    private static readonly ConcurrentDictionary<Type, object> serviceInstances = [];

    public static IEnumerable<Type> GetAllServiceTypes<T>(this IServiceProvider serviceProvider)
    {
        return serviceTypes.GetOrAdd(typeof(T), type =>
        {
            var serviceCollection = serviceProvider.GetRequiredService<IServiceCollection>();
            var list = new List<Type>();

            foreach (var item in serviceCollection)
            {
                if (item.ServiceType != typeof(T)) continue;

                if (item.IsKeyedService && item.KeyedImplementationType != null)
                {
                    list.Add(item.KeyedImplementationType);
                }
                else if (item.ImplementationType != null)
                {
                    list.Add(item.ImplementationType);
                }
            }

            return list;
        });
    }

    public static IEnumerable<T> GetAllServices<T>(this IServiceProvider serviceProvider)
    {
        var result = serviceInstances.GetOrAdd(typeof(T), type =>
        {
            var serviceCollection = serviceProvider.GetRequiredService<IServiceCollection>();
            var list = new List<T>();
            var keys = new HashSet<object>();

            foreach (var item in serviceCollection)
            {
                if (item.ServiceType != typeof(T)) continue;
                if (item.IsKeyedService && item.ServiceKey != null)
                {
                    keys.Add(item.ServiceKey);
                }
            }

            list.AddRange(serviceProvider.GetServices<T>());

            foreach (var item in keys)
            {
                list.AddRange(serviceProvider.GetKeyedServices<T>(item));
            }

            return list;
        }) as IEnumerable<T>;

        return result ?? [];
    }

    public static IServiceProvider GetHttpScope(this IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        if (httpContextAccessor?.HttpContext == null) throw new HttpScopeNotAvailableException();
        return httpContextAccessor.HttpContext.RequestServices;
    }
}

#endif