using System;
using System.IO;
using Castle.DynamicProxy;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WiseInterceptors.Common;

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
            _helper.GetDefaultValue(typeof(MemoryStream)).Should().Be(null);
        }
    }
}
