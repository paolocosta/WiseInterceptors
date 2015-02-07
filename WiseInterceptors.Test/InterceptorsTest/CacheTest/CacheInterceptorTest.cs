using Autofac;
using Autofac.Extras.DynamicProxy2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Cache;
using WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest;
using FluentAssertions;
using WiseInterceptor;
using NSubstitute;
using Castle.DynamicProxy;
using WiseInterceptor.Common;
using System.Reflection;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    [TestFixture]
    [Category("Cache")]
    public class CacheInterceptorTest
    {
        [Test]
        public void should_call_get_settings()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<InterceptorModule>();

            var cache = Substitute.For<ICache>();
            cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new CacheSettings { UseCache=false});
            builder.Register(c => cache).As<ICache>();
            builder.RegisterType<Cachable>()
            .EnableClassInterceptors()
            .InterceptedBy(typeof(CacheInterceptor));

            var container = builder.Build();

            var cachable = container.Resolve<Cachable>();
            
            cachable.DoNothing();
            
            cache.Received().GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>());
        }

        
    }

    public class Cachable
    {
        public virtual void DoNothing()
        {
            
        }

        public virtual string Hello(string Name)
        {
            return string.Format("Hello {Name}", Name);
        }
    }
}
