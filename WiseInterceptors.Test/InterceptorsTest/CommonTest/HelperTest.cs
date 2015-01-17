using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Castle.DynamicProxy;
using WiseInterceptor.Interceptors.Common;
using FluentAssertions;

namespace WiseInterceptors.Test.InterceptorsTest.CommonTest
{
    [TestFixture]
    class HelperTest
    {
        [Test]
        public void GetCallIdentifier_returns_correct_value()
        {
            var invocation = Substitute.For<IInvocation>();
            invocation.Arguments.Returns(new object[] { 1, 2, 3 });
            invocation.Method.Returns(typeof(DateTime).GetMethod("FromOADate"));

            var helper = new Helper();
            helper.GetCallIdentifier(invocation).Should().Be("System.DateTime_FromOADate_[1,2,3]");
        }

        [Test]
        public void GetMethodIdentifier_returns_correct_value()
        {
            var invocation = Substitute.For<IInvocation>();
            invocation.Method.Returns(typeof(DateTime).GetMethod("FromOADate"));

            var helper = new Helper();
            helper.GetMethodIdentifier(invocation).Should().Be("System.DateTime_FromOADate");
        }
    }
}
