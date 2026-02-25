using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace VoidNone.DependencyInjectionTest;

[TestClass]
public class ShareInstanceTest
{
    interface IService1 { }
    interface IService2 : IService1 { }
    interface IService3 : IService2 { }
    interface IService4 : IService3 { }

    [Singleton<IService1, IService2>]
    [Singleton<IService3, IService4>(nameof(Service))]
    class Service : IService4 { }

    IServiceProvider _services = new ServiceCollection().AddFromAssemblies(Assembly.GetExecutingAssembly()).BuildServiceProvider();

    [TestMethod]
    public void LifetimeAttribute()
    {
        var service1 = _services.GetService<IService1>();
        var service2 = _services.GetService<IService2>();
        var service3 = _services.GetKeyedService<IService3>(nameof(Service));
        var service4 = _services.GetKeyedService<IService4>(nameof(Service));
        Assert.AreEqual(service2, service1);
        Assert.AreNotEqual(service2, service3);
        Assert.AreEqual(service3, service4);
    }
}