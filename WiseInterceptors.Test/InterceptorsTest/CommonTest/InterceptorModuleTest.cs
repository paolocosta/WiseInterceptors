using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;
using FluentAssertions;
using NUnit.Framework;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.CircuitBreaker;
using WiseInterceptors.Interceptors.MethodValidation;
using NSubstitute;

namespace WiseInterceptors.Test.InterceptorsTest.CommonTest
{
    public class InterceptorModuleTest
    {
        //TODO Create a Setup method and extract common varialbles
        [Test]
        public void should_resolve_IHelper()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<InterceptorModule>();
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            var helper = scope.Resolve<IHelper>();
            helper.GetType().Should().Be(typeof(Helper));
        }

        [Test]
        public void should_resolve_ICacheInvocationManagerFactory()
        {
            var builder = new ContainerBuilder();
            var cache = Substitute.For<ICache>();
            builder.RegisterType(cache.GetType()).As(typeof(ICache));
            builder.RegisterModule<InterceptorModule>();
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            var helper = scope.Resolve<ICacheInvocationManagerFactory>();
            helper.GetType().Should().Be(typeof(CacheInvocationManagerFactory));
        }

        [Test]
        public void should_resolve_CacheInterceptor()
        {
            var builder = new ContainerBuilder();
            var cache = Substitute.For<ICache>();
            builder.RegisterType(cache.GetType()).As(typeof(ICache));
            builder.RegisterModule<InterceptorModule>();
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            var helper = scope.Resolve<CacheInterceptor>();
            helper.GetType().Should().Be(typeof(CacheInterceptor));
        }

        [Test]
        public void should_resolve_CircuitBreakerInterceptor()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<InterceptorModule>();
            var cache = Substitute.For<ICache>();
            var circuitBreakerSettingsReader = Substitute.For<ICircuitBreakerSettingsReader>();
            builder.RegisterType(circuitBreakerSettingsReader.GetType()).As(typeof(ICircuitBreakerSettingsReader));
            builder.RegisterType(cache.GetType()).As(typeof(ICache));
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            var helper = scope.Resolve<CircuitBreakerInterceptor>();
            helper.GetType().Should().Be(typeof(CircuitBreakerInterceptor));
        }

        [Test]
        public void should_resolve_MethodValidationInterceptor()
        {
            var builder = new ContainerBuilder();
            var resolver = Substitute.For<IMethodValidationSettingsResolver>();

            builder.RegisterModule<InterceptorModule>();
            builder.RegisterType(resolver.GetType()).As(typeof(IMethodValidationSettingsResolver));
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            var helper = scope.Resolve<MethodValidationInterceptor>();
            helper.GetType().Should().Be(typeof(MethodValidationInterceptor));
        }
    }
}
