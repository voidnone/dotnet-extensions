using System;

namespace Microsoft.Extensions.DependencyInjection;

public class SingletonAttribute : LifetimeAttribute
{
    public SingletonAttribute(params Type[] services) : base(ServiceLifetime.Singleton, services)
    {
    }

#if NET8_0_OR_GREATER
    public SingletonAttribute(object? key, params Type[] services) : base(key, ServiceLifetime.Singleton, services)
    {
    }
#endif

}

#if NET8_0_OR_GREATER

public class SingletonAttribute<TService> : SingletonAttribute
{
    public SingletonAttribute() : base([typeof(TService)])
    {
    }

    public SingletonAttribute(object? key) : base(key, [typeof(TService)])
    {
    }
}

public class SingletonAttribute<TService1, TService2> : SingletonAttribute
{
    public SingletonAttribute() : base([typeof(TService1), typeof(TService2)])
    {
    }

    public SingletonAttribute(object? key) : base(key, [typeof(TService1), typeof(TService2)])
    {
    }
}

public class SingletonAttribute<TService1, TService2, TService3> : SingletonAttribute
{
    public SingletonAttribute() : base([typeof(TService1), typeof(TService2), typeof(TService3)])
    {
    }

    public SingletonAttribute(object? key) : base(key, [typeof(TService1), typeof(TService2), typeof(TService3)])
    {
    }
}

#endif

