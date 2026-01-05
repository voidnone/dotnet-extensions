using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace DependencyInjectionTest
{

    [TestClass]
    public class GenericsServiceTest
    {
        [Transient(typeof(IService<>))]
        class Service<T> : IService<T> { }

        interface IService<T> { }

        interface IMyService : IService<DateTime> { }

        [Transient(typeof(IMyService))]
        class MyService : Service<DateTime>, IMyService { }


        IServiceProvider _services = new ServiceCollection().AddFromAssemblies(Assembly.GetExecutingAssembly()).BuildServiceProvider();

        [TestMethod]
        public void Has_Interface_Service_Test()
        {
            var service = _services.GetService<IService<DateTime>>();
            var service2 = _services.GetService<IMyService>();
            Assert.IsInstanceOfType(service, typeof(Service<DateTime>));
            Assert.IsInstanceOfType(service2, typeof(MyService));
        }
    }
}
