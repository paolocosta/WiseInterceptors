﻿using Autofac;
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
        public void should_call_get_result()
        {
            var cache = Substitute.For<ICache>();
            var helper = Substitute.For<IHelper>();
            var invocationManager = Substitute.For<ICacheInvocationManager>();
            var invocation = Substitute.For<IInvocation>();
            invocationManager.GetResult(invocation).Returns(1);
            var sut = new CacheInterceptor(cache, helper, invocationManager);
            sut.Intercept(invocation);

            invocation.ReturnValue.Should().Be(1);
        }
    }
}