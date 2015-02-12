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

namespace WiseInterceptors.Test.InterceptorsTest.CommonTest
{
    [TestFixture]
    class HelperTest
    {
        [Test]
        public void should_GetCallIdentifier_return_correct_value()
        {
            var invocation = Substitute.For<IInvocation>();
            invocation.Arguments.Returns(new object[] { 1, 2, 3 });
            invocation.GetConcreteMethodInvocationTarget().Returns(typeof(DateTime).GetMethod("FromOADate"));

            var helper = new Helper();
            helper.GetUnivoqueCallIdentifier(invocation).Should().Be("System.DateTime_FromOADate_[1,2,3]");
        }

        [Test]
        public void should_GetMethodIdentifier_return_correct_value()
        {
            var invocation = Substitute.For<IInvocation>();
            invocation.GetConcreteMethodInvocationTarget().Returns(typeof(DateTime).GetMethod("FromOADate"));

            var helper = new Helper();
            helper.GetMethodIdentifier(invocation).Should().Be("System.DateTime_FromOADate");
        }

        [Test]
        public void should_GetDefaultValue_return_0_for_a_value_type()
        {
            var sut = new Helper();
            sut.GetDefaultValue(typeof(Int32)).Should().Be(0);
        }

        [Test]
        public void should_GetDefaultValue_return_null_for_a_reference_type()
        {
            var sut = new Helper();
            sut.GetDefaultValue(typeof(System.IO.MemoryStream)).Should().Be(null);
        }
    }
}
