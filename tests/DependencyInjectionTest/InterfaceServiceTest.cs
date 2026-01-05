using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace DependencyInjectionTest
{
    [TestClass]
    public class InterfaceServiceTest
    {
        [Transient(typeof(IService), typeof(IService2))]
        class Service : IService, IService2 { }


        [Transient<IService3>, Transient<IService4, IService5, IService6>]
        class Service2 : IService3, IService4, IService5, IService6 { }

        interface IService { }

        interface IService2 { }

        interface IService3 { }

        interface IService4 { }
        interface IService5 { }
        interface IService6 { }


        IServiceProvider _services = new ServiceCollection().AddFromAssemblies(Assembly.GetExecutingAssembly()).BuildServiceProvider();

        [TestMethod]
        public void Has_Interface_Service_Test()
        {
            var service = _services.GetService<IService>();
            var service2 = _services.GetService<IService2>();
            Assert.IsInstanceOfType(service, typeof(Service));
            Assert.IsInstanceOfType(service2, typeof(Service));
        }

        [TestMethod]
        public void Generic_Interface_Service_Test()
        {
            var service = _services.GetService<IService3>();
            var service2 = _services.GetService<IService4>();
            var service3 = _services.GetService<IService5>();
            var service4 = _services.GetService<IService6>();
            Assert.IsInstanceOfType(service, typeof(Service2));
            Assert.IsInstanceOfType(service2, typeof(Service2));
            Assert.IsInstanceOfType(service3, typeof(Service2));
            Assert.IsInstanceOfType(service4, typeof(Service2));
        }

        [TestMethod]
        public void Not_Class_Service_Test()
        {
            var service = _services.GetService<Service>();
            Assert.IsNull(service);
        }
    }
}
