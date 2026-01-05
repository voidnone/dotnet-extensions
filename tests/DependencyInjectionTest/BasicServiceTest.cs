using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace DependencyInjectionTest
{
    [TestClass]
    public class BasicServiceTest
    {
        [Lifetime(ServiceLifetime.Transient)]
        class LifetimeService { }

        [Lifetime("key", ServiceLifetime.Transient)]
        class KeyedService { }

        [Transient]
        class TransientService { }

        [Scoped]
        class ScopedService { }

        [Singleton]
        class SingletonService { }

        IServiceProvider _services = new ServiceCollection().AddFromAssemblies(Assembly.GetExecutingAssembly()).BuildServiceProvider();

        [TestMethod]
        public void LifetimeAttributeTest()
        {
            var service = _services.GetService<LifetimeService>();
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void KeyedServiceTest()
        {
            var nonKeyService = _services.GetService<KeyedService>();
            Assert.IsNull(nonKeyService);
            var keyedService = _services.GetKeyedService<KeyedService>("key");
            Assert.IsNotNull(keyedService);
            var wrongKeyService = _services.GetKeyedService<KeyedService>("wrong_key");
            Assert.IsNull(wrongKeyService);
        }

        [TestMethod]
        public void TransientAttributeTest()
        {
            var service = _services.GetService<TransientService>();
            var service2 = _services.GetService<TransientService>();
            Assert.IsNotNull(service);
            Assert.IsNotNull(service2);
            Assert.AreNotEqual(service, service2);
        }

        [TestMethod]
        public void ScopedAttributeTest()
        {
            var service = _services.GetService<ScopedService>();
            var service2 = _services.GetService<ScopedService>();
            Assert.IsNotNull(service);
            Assert.IsNotNull(service2);
            Assert.AreEqual(service, service2);
        }

        [TestMethod]
        public void SingletonAttributeTest()
        {
            var service = _services.GetService<SingletonService>();
            var service2 = _services.GetService<SingletonService>();
            Assert.IsNotNull(service);
            Assert.IsNotNull(service2);
            Assert.AreEqual(service, service2);
        }
    }
}
