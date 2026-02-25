using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace VoidNone.DependencyInjectionTest;

[TestClass]
public class ServiceProviderExtensionsTest
{
    interface IService { }

    [Singleton<IService>]
    class Service : IService { }

    [Singleton<IService>("key1")]
    class Service1 : IService { }

    [Singleton<IService>("key2")]
    class Service2 : IService { }
    
    IServiceProvider _services = new ServiceCollection().AddFromAssemblies(Assembly.GetExecutingAssembly()).BuildServiceProvider();

    [TestMethod]
    public void GetAllServiceTypes()
    {
        var types = _services.GetAllServiceTypes<IService>();
        Assert.AreEqual(3, types.Count());
        Assert.IsTrue(types.Contains(typeof(Service)));
        Assert.IsTrue(types.Contains(typeof(Service1)));
        Assert.IsTrue(types.Contains(typeof(Service2)));
    }

    [TestMethod]
    public void GetAllServices()
    {
        var instances = _services.GetAllServices<IService>();
        Assert.AreEqual(3, instances.Count());
        Assert.IsTrue(instances.Any(a => a is Service));
        Assert.IsTrue(instances.Any(a => a is Service1));
        Assert.IsTrue(instances.Any(a => a is Service2));
    }
}