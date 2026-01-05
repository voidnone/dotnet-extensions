using System;

namespace Microsoft.Extensions.DependencyInjection;

public class TransientAttribute : LifetimeAttribute
{
    public TransientAttribute(params Type[] services) : base(ServiceLifetime.Transient, services)
    {
    }

#if NET8_0_OR_GREATER
    public TransientAttribute(object? key, params Type[] services) : base(key, ServiceLifetime.Transient, services)
    {
    }
#endif

}

#if NET8_0_OR_GREATER
public class TransientAttribute<TService> : TransientAttribute
{
    public TransientAttribute() : base([typeof(TService)])
    {
    }


    public TransientAttribute(object? key) : base(key, [typeof(TService)])
    {
    }
}

public class TransientAttribute<TService1, TService2> : TransientAttribute
{
    public TransientAttribute() : base([typeof(TService1), typeof(TService2)])
    {
    }

    public TransientAttribute(object? key) : base(key, [typeof(TService1), typeof(TService2)])
    {
    }
}

public class TransientAttribute<TService1, TService2, TService3> : TransientAttribute
{
    public TransientAttribute() : base([typeof(TService1), typeof(TService2), typeof(TService3)])
    {
    }

    public TransientAttribute(object? key) : base(key, [typeof(TService1), typeof(TService2), typeof(TService3)])
    {
    }
}

#endif


