using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjectionTest;

[TestClass]
public class ServiceCollectionExtensionsTest
{
    interface IService1 { }
    interface IService2 : IService1 { }
    interface IService3 : IService2 { }
    interface IService4 : IService3 { }

    [TestMethod]
    public void GetAllServices()
    {
        var list = new List<Type>
        {
            typeof(IService3),
            typeof(IService4),
            typeof(IService1),
            typeof(IService2),
        };


        var result = ServiceCollectionExtensions.SortTypeByAssignable([.. list]).ToArray();
        Assert.AreEqual(typeof(IService4), result[0]);
        Assert.AreEqual(typeof(IService3), result[1]);
        Assert.AreEqual(typeof(IService2), result[2]);
        Assert.AreEqual(typeof(IService1), result[3]);
    }
}
