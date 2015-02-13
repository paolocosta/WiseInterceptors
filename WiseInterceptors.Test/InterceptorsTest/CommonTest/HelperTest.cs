using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Castle.DynamicProxy;
using FluentAssertions;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.Cache.Strategies;

namespace WiseInterceptors.Test.InterceptorsTest.CommonTest
{
    [TestFixture]
    class HelperTest
    {
        private Helper _helper;

        [SetUp]
        public void Setup()
        {
            var cache = Substitute.For<ICache>();
            _helper = new Helper();
        }

        [Test]
        public void should_GetCallIdentifier_return_correct_value()
        {
            var invocation = Substitute.For<IInvocation>();
            invocation.Arguments.Returns(new object[] { 1, 2, 3 });
            invocation.GetConcreteMethodInvocationTarget().Returns(typeof(DateTime).GetMethod("FromOADate"));

            _helper.GetUnivoqueCallIdentifier(invocation).Should().Be("System.DateTime_FromOADate_[1,2,3]");
        }

        [Test]
        public void should_GetMethodIdentifier_return_correct_value()
        {
            var invocation = Substitute.For<IInvocation>();
            invocation.GetConcreteMethodInvocationTarget().Returns(typeof(DateTime).GetMethod("FromOADate"));

            _helper.GetMethodIdentifier(invocation).Should().Be("System.DateTime_FromOADate");
        }

        [Test]
        public void should_GetDefaultValue_return_0_for_a_value_type()
        {
            _helper.GetDefaultValue(typeof(Int32)).Should().Be(0);
        }

        [Test]
        public void should_GetDefaultValue_return_null_for_a_reference_type()
        {
            _helper.GetDefaultValue(typeof(System.IO.MemoryStream)).Should().Be(null);
        }

        //[Test]
        //[TestCase(FaultToleranceEnum.AlwaysUsePersistentCache, typeof(AlwaysUsePersistentCacheInvocationManager))]
        //[TestCase(FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors, typeof(ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager))]
        //[TestCase(FaultToleranceEnum.FailFastWithNoRecovery, typeof(FailFastCacheInvocationManager))]
        //[TestCase(FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError, typeof(UsePersistentCacheOnlyInCaseOfErrorInvocationManager))]
        //public void should_GetCacheInvocationManagerImplementation_retrn_the_correct_result(FaultToleranceEnum faultTolerance, Type cacheInvocationManagerType)
        //{
        //    var cacheInvocationManager = _helper.GetCacheInvocationManagerImplementation(faultTolerance, cacheInvocationManagerType);
        //    cacheInvocationManager.Should().BeOfType(cacheInvocationManagerType);
        //}
    }
}
