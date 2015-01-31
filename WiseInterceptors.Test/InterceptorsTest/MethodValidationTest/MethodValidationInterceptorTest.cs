using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Castle.DynamicProxy;
using System.Reflection;
using NUnit.Framework;
using FluentAssertions;
using WiseInterceptor.Interceptors.MethodValidation;

namespace WiseInterceptors.Test.InterceptorsTest.MethodValidationTest
{
    public class MethodValidationInterceptorTest
    {
        private MethodValidationInterceptor _sut;
        private IInvocation _invocation;

        [SetUp]
        public void Setup()
        {
            _sut = new MethodValidationInterceptor();
            _invocation = Substitute.For<IInvocation>();
        }

        [Test]
        public void should_not_throw_exception_when_method_has_no_parameters()
        {
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithNoParameters"));

            _sut.Intercept(_invocation);

            _invocation.Received().Proceed();
        }

        [Test]
        public void should_not_throw_exception_when_BlockDefaultValues_is_defined_and_all_paameter_values_are_not_default()
        {
            _invocation.Arguments.Returns(new object[] { 1, 2, 3 });
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithBlockDefaultValuesAttribute"));

            _sut.Intercept(_invocation);

            _invocation.Received().Proceed();
        }

        [Test]
        public void should_throw_exception_when_BlockDefaultValues_is_defined_and_at_least_one_parameter_value_is_default()
        {
            _invocation.Arguments.Returns(new object[] { 0, 2, 3 });
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithBlockDefaultValuesAttribute"));

            try
            {
                _sut.Intercept(_invocation);
                
                Assert.Fail();
            }
            catch (DefaultParameterValuePreConditionException ex)
            {
                ex.Message.Should().Contain("Method MethodWithBlockDefaultValuesAttribute of type MethodValidationInterceptorTestHelper is called with default value on parameter firstParameter");
            }
        }

        [Test]
        public void should_not_throw_exception_when_BlockDefaultValues_is_not_defined_and_more_than_one_parameter_value_is_default()
        {
            _invocation.Arguments.Returns(new object[] { 0, 2, 3 });
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithoutBlockDefaultValuesAttribute"));

            _sut.Intercept(_invocation);
        }

        [Test]
        public void should_throw_BlockDefaultResultPostConditionException_when_method_is_decorated_with_BlockDefaultResultAttribute_and_default_value_is_returned()
        {

            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithBlockDefaultResultAttribute"));
            _invocation.ReturnValue = 0;
            
            try
            {
                _sut.Intercept(_invocation);
                
                Assert.Fail();
            }
            catch (DefaultResultPostConditionException ex)
            {
                ex.Message.Should().Contain("Method MethodWithBlockDefaultResultAttribute of type MethodValidationInterceptorTestHelper cannot return default value");
            }
        }

        [Test]
        public void should_not_throw_exception_when_method_is_not_decorated_with_BlockDefaultResultAttribute_and_default_value_is_returned()
        {
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithoutBlockDefaultResultAttribute"));
            _invocation.ReturnValue = 0;
            
            _sut.Intercept(_invocation);
        }
    }

    public class MethodValidationInterceptorTestHelper
    {
        [BlockDefaultParameterValues]
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

        public int MethodWithoutBlockDefaultResultAttribute(int firstParameter, int secondParameter)
        {
            return 0;
        }

        [BlockDefaultParameterValues]
        public void MethodWithNoParameters()
        {
        }
    }
}

