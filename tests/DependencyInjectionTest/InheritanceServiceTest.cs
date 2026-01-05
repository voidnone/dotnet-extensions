using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace DependencyInjectionTest
{
    [TestClass]
    public class InheritanceServiceTest
    {
        [Transient(typeof(IBaseService))]
        class Service : BaseService, IService { };

        [Transient]
        class BaseService { };
        interface IService : IBaseService { };
        interface IBaseService { };

        IServiceProvider _services = new ServiceCollection().AddFromAssemblies(Assembly.GetExecutingAssembly()).BuildServiceProvider();

        [TestMethod]
        public void Class_Test()
        {
            var service = _services.GetService<Service>();
            var baseService = _services.GetService<BaseService>();

            Assert.IsNull(service);
            Assert.IsNotNull(baseService);
        }

        [TestMethod]
        public void Interface_Test()
        {
            var service = _services.GetService<IService>();
            var baseService = _services.GetService<IBaseService>();

            Assert.IsNull(service);
            Assert.IsNotNull(baseService);
            Assert.IsInstanceOfType(baseService, typeof(Service));
        }
    }
}
