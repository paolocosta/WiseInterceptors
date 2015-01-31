using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Castle.DynamicProxy;
using System.Reflection;
using NUnit.Framework;
using WiseInterceptor.Interceptors.Defensive;
using FluentAssertions;
using WiseInterceptor.Utilities;

namespace WiseInterceptors.Test.InterceptorsTest.DefensiveTest
{
    public class DefensiveInterceptorTest
    {
        private DefensiveInterceptor _sut;
        private IInvocation _invocation;
        
        [SetUp]
        public void Setup()
        {
            _sut = new DefensiveInterceptor();
            _invocation = Substitute.For<IInvocation>();
        }

        [Test]
        public void should_not_throw_exception_when_method_has_no_parameters()
        {
            _invocation.MethodInvocationTarget.Returns(typeof(DefensiveInterceptorTestHelper).GetMethod("MethodWithNoParameters"));

            _sut.Intercept(_invocation);

            _invocation.Received().Proceed();
        }

        [Test]
        public void should_not_throw_exception_when_BlockDefaultValues_is_defined_and_all_paameter_values_are_not_default()
        {
            _invocation.Arguments.Returns(new object[] { 1, 2, 3 });

            _invocation.MethodInvocationTarget.Returns(typeof(DefensiveInterceptorTestHelper).GetMethod("MethodWithBlockDefaultValuesAttribute"));

            _sut.Intercept(_invocation);

            _invocation.Received().Proceed();
        }

        [Test]
        public void should_throw_exception_when_BlockDefaultValues_is_defined_and_at_least_one_parameter_value_is_default()
        {
            try
            {
                _invocation.Arguments.Returns(new object[] { 0, 2, 3 });
                _invocation.MethodInvocationTarget.Returns(typeof(DefensiveInterceptorTestHelper).GetMethod("MethodWithBlockDefaultValuesAttribute"));

                _sut.Intercept(_invocation);
                Assert.Fail();
            }
            catch (BlockDefaultValuePreconditionViolatedException ex)
            {
                ex.Message.Should().Contain("Method MethodWithBlockDefaultValuesAttribute of type DefensiveInterceptorTestHelper is called with default value on parameter firstParameter");
            }
        }

        [Test]
        public void should_not_throw_exception_when_BlockDefaultValues_is_not_defined_and_more_than_one_parameter_value_is_default()
        {            
            _invocation.Arguments.Returns(new object[] { 0, 2, 3 });
            _invocation.MethodInvocationTarget.Returns(typeof(DefensiveInterceptorTestHelper).GetMethod("MethodWithoutBlockDefaultValuesAttribute"));

            _sut.Intercept(_invocation);
        }

        [Test]
        public void should_throw_BlockDefaultResultPostConditionException_when_method_is_decorated_with_BlockDefaultResultAttribute_and_default_value_is_returned()
        {
            try
            {
                _invocation.MethodInvocationTarget.Returns(typeof(DefensiveInterceptorTestHelper).GetMethod("MethodWithBlockDefaultResultAttribute"));
                _invocation.ReturnValue = 0;
                _sut.Intercept(_invocation);
                Assert.Fail();
            }
            catch (NoDefaultResultPostConditionException ex)
            {
                ex.Message.Should().Contain("Method MethodWithBlockDefaultResultAttribute of type DefensiveInterceptorTestHelper cannot return default value");
            }
        }
    }

    public class DefensiveInterceptorTestHelper
    {
        [BlockDefaultValues]
        public void MethodWithBlockDefaultValuesAttribute(int firstParameter, int secondParameter)
        { 
        
        }

        [BlockDefaultResult]
        public int MethodWithBlockDefaultResultAttribute(int firstParameter, int secondParameter)
        {
            return 0;
        }

        public void MethodWithoutBlockDefaultValuesAttribute(int firstParameter, int secondParameter)
        {

        }

        public void MethodWithNoParameters()
        {

        }
    }
}

