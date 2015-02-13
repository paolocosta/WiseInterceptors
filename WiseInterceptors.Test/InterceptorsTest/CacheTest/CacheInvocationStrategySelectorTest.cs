using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Interceptors.Cache;
using NUnit.Framework;
using WiseInterceptors.Interceptors.Cache.Strategies;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using NSubstitute;
using WiseInterceptors.Common;
using Castle.DynamicProxy;
using FluentAssertions;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    public class CacheInvocationStrategySelectorTest
    {
        [Test]
        [TestCase(FaultToleranceEnum.AlwaysUsePersistentCache, typeof(AlwaysUsePersistentCacheInvocationManager))]
        [TestCase(FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors, typeof(ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager))]
        [TestCase(FaultToleranceEnum.FailFastWithNoRecovery, typeof(FailFastCacheInvocationManager))]
        [TestCase(FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError, typeof(UsePersistentCacheOnlyInCaseOfErrorInvocationManager))]
        public void should_build_the_right_manager_according_to_FaultToleranceStrategy(FaultToleranceEnum faultTolerance, Type expectedManagerType)
        { 
            var cache = Substitute.For<ICache>();
            var helper = Substitute.For<IHelper>();
            var invocation = Substitute.For<IInvocation>();
            cache.GetFaultToleranceStrategy().Returns(faultTolerance);
            var sut = new CacheInvocationStrategySelector(cache, helper);
            var manager = sut.GetCacheManagerImplementation();
            manager.GetType().Should().Be(expectedManagerType);
        }
    }
}
