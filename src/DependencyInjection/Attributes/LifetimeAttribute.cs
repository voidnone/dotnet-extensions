using System;

namespace Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class LifetimeAttribute : Attribute
{
    public LifetimeAttribute(ServiceLifetime serviceLifetime, params Type[] services)
    {
        ServiceLifetime = serviceLifetime;
        Services = services;
    }

#if NET8_0_OR_GREATER
    public LifetimeAttribute(object? key, ServiceLifetime serviceLifetime, params Type[] services)
    {
        Key = key;
        ServiceLifetime = serviceLifetime;
        Services = services;
    }
#endif

    public object? Key { get; private set; }
    public ServiceLifetime ServiceLifetime { get; private set; }
    public Type[] Services { get; private set; }
}

#if NET8_0_OR_GREATER

public class LifetimeAttribute<TService> : LifetimeAttribute
{
    public LifetimeAttribute(ServiceLifetime serviceLifetime) : base(serviceLifetime, [typeof(TService)])
    {
    }


    public LifetimeAttribute(object? key, ServiceLifetime serviceLifetime) : base(key, serviceLifetime, [typeof(TService)])
    {
    }
}

public class LifetimeAttribute<TService1, TService2> : LifetimeAttribute
{
    public LifetimeAttribute(ServiceLifetime serviceLifetime) : base(serviceLifetime, [typeof(TService1), typeof(TService2)])
    {
    }

    public LifetimeAttribute(object? key, ServiceLifetime serviceLifetime) : base(key, serviceLifetime, [typeof(TService1), typeof(TService2)])
    {
    }
}

public class LifetimeAttribute<TService1, TService2, TService3> : LifetimeAttribute
{
    public LifetimeAttribute(ServiceLifetime serviceLifetime) : base(serviceLifetime, [typeof(TService1), typeof(TService2), typeof(TService3)])
    {
    }

    public LifetimeAttribute(object? key, ServiceLifetime serviceLifetime) : base(key, serviceLifetime, [typeof(TService1), typeof(TService2), typeof(TService3)])
    {
    }
}

#endif