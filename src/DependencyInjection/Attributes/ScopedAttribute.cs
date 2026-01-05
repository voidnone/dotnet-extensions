using System;

namespace Microsoft.Extensions.DependencyInjection;

public class ScopedAttribute : LifetimeAttribute
{
    public ScopedAttribute(params Type[] services) : base(ServiceLifetime.Scoped, services)
    {

    }

#if NET8_0_OR_GREATER
    public ScopedAttribute(object? key, params Type[] services) : base(key, ServiceLifetime.Scoped, services)
    {

    }
#endif

}

#if NET8_0_OR_GREATER

public class ScopedAttribute<TService> : ScopedAttribute
{
    public ScopedAttribute() : base([typeof(TService)])
    {

    }

    public ScopedAttribute(object? key) : base(key, [typeof(TService)])
    {

    }
}

public class ScopedAttribute<TService1, TService2> : ScopedAttribute
{
    public ScopedAttribute() : base([typeof(TService1), typeof(TService2)])
    {

    }

    public ScopedAttribute(object? key) : base(key, [typeof(TService1), typeof(TService2)])
    {

    }
}

public class ScopedAttribute<TService1, TService2, TService3> : ScopedAttribute
{
    public ScopedAttribute() : base([typeof(TService1), typeof(TService2), typeof(TService3)])
    {

    }

    public ScopedAttribute(object? key) : base(key, [typeof(TService1), typeof(TService2), typeof(TService3)])
    {

    }
}

#endif
