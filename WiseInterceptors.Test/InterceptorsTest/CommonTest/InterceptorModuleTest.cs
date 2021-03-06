﻿using System;
using System.Reflection;
using Autofac;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using WiseInterceptors.Interceptors.CircuitBreaker;
using WiseInterceptors.Interceptors.MethodValidation;
using WiseInterceptors.Interceptors.Cache.Strategies;

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
        public void should_resolve_ICacheInvocationStrategySelector()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DummyCache>().As<ICache>();
            builder.RegisterModule<InterceptorModule>();
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            var helper = scope.Resolve<ICacheInvocationStrategySelector>();
            helper.GetType().Should().Be(typeof(CacheInvocationStrategySelector));
        }

        [Test]
        public void should_resolve_CacheInterceptor()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DummyCache>().As<ICache>();
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
        [TestCase(FaultToleranceEnum.AlwaysUsePersistentCache, typeof(AlwaysUsePersistentCacheInvocationManager))]
        [TestCase(FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors, typeof(ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager))]
        [TestCase(FaultToleranceEnum.FailFastWithNoRecovery, typeof(FailFastCacheInvocationManager))]
        [TestCase(FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError, typeof(UsePersistentCacheOnlyInCaseOfErrorInvocationManager))]
        public void should_resolve_ICacheInvocationStrategySelector(FaultToleranceEnum faultTolerance, Type expected)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<InterceptorModule>();
            var cache = Substitute.For<ICache>();
            builder.Register(c=> cache).As<ICache>();
            cache.GetFaultToleranceStrategy().Returns(faultTolerance);
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            
            var sut = scope.Resolve<ICacheInvocationStrategySelector>();
            sut.GetCacheManagerImplementation().Should().BeOfType(expected);
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

    internal class DummyCache : ICache
    {
        public void Insert(string Key, object Value, DateTime Expiration)
        {
            throw new NotImplementedException();
        }

        public object Get(string Key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string Key)
        {
            throw new NotImplementedException();
        }

        public void InsertInPersistentCache(string Key, object Value)
        {
            throw new NotImplementedException();
        }

        public object GetFromPersistentCache(string Key)
        {
            throw new NotImplementedException();
        }

        public CacheSettings GetSettings(MethodInfo method, object[] arguments)
        {
            throw new NotImplementedException();
        }

        public FaultToleranceEnum GetFaultToleranceStrategy()
        {
            return FaultToleranceEnum.FailFastWithNoRecovery;
        }
    }
}
