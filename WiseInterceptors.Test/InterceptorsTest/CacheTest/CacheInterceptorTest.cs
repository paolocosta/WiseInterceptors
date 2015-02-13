using Autofac;
using Autofac.Extras.DynamicProxy2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest;
using FluentAssertions;
using WiseInterceptors;
using NSubstitute;
using Castle.DynamicProxy;
using WiseInterceptors.Common;
using System.Reflection;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    [TestFixture]
    [Category("Cache")]
    public class CacheInterceptorTest
    {
        ///TODO ALL CACHE INVOCATION MANAGERS ARE TO TESTED
        [Test]
        public void should_Intercept_call_build()
        {
            var cache = Substitute.For<ICache>();
            var helper = Substitute.For<IHelper>();
            var invocation = Substitute.For<IInvocation>();
            var invocationManagerFactory = Substitute.For<ICacheInvocationManagerFactory>();
            invocationManagerFactory.Build(invocation).Returns(Substitute.For<ICacheInvocationManager>());
            var sut = new CacheInterceptor(cache, helper, invocationManagerFactory);
            sut.Intercept(invocation);

            invocationManagerFactory.Received().Build(invocation);
        }
    }
}
