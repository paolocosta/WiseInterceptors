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
        private IMethodValidationSettingsResolver _methodValidationSettingsResolver;

        [SetUp]
        public void Setup()
        {
            _methodValidationSettingsResolver = Substitute.For<IMethodValidationSettingsResolver>();
            _sut = new MethodValidationInterceptor(_methodValidationSettingsResolver);
            _invocation = Substitute.For<IInvocation>();
        }

        [Test]
        public void should_not_throw_exception_when_method_has_no_parameters()
        {
            _methodValidationSettingsResolver.BlockDefaultValueParameters(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(true);
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodWithNoParameters"));

            _sut.Intercept(_invocation);

            _invocation.Received().Proceed();
        }

        [Test]
        public void should_not_throw_exception_when_BlockDefaultValueParameters_returns_true_and_all_parameter_values_are_not_default()
        {
            _methodValidationSettingsResolver.BlockDefaultValueParameters(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(true);
            _invocation.Arguments.Returns(new object[] { 1, 2 });
            //_invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("VoidMethod"));

            _sut.Intercept(_invocation);

            _invocation.Received().Proceed();
        }

        [Test]
        public void should_throw_exception_when_BlockDefaultValueParameters_returns_true_and_at_least_one_parameter_value_is_default()
        {
            _methodValidationSettingsResolver.BlockDefaultValueParameters(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(true);   
            _invocation.Arguments.Returns(new object[] { 0, 2 });
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("VoidMethod"));

            try
            {
                _sut.Intercept(_invocation);
                
                Assert.Fail();
            }
            catch (DefaultParameterValuePreConditionException ex)
            {
                ex.Message.Should().Contain("Method VoidMethod of type MethodValidationInterceptorTestHelper is called with default value on parameter p1");
            }
        }

        [Test]
        public void should_not_throw_exception_when_BlockDefaultValueParameters_returns_false_and_more_than_one_parameter_value_is_default()
        {
            _methodValidationSettingsResolver.BlockDefaultValueParameters(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(false);
            _invocation.Arguments.Returns(new object[] { 0, 2, 3 });
            
            _sut.Intercept(_invocation);
        }

        [Test]
        public void should_throw_BlockDefaultResultPostConditionException_when_BlockDefaultResult_returns_true_and_default_value_is_returned()
        {
            _methodValidationSettingsResolver.BlockDefaultResult(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(true);
            _invocation.MethodInvocationTarget.Returns(typeof(MethodValidationInterceptorTestHelper).GetMethod("MethodThatReturnsDefaultValue"));
            _invocation.ReturnValue = 0;
            
            try
            {
                _sut.Intercept(_invocation);
                
                Assert.Fail();
            }
            catch (DefaultResultPostConditionException ex)
            {
                ex.Message.Should().Contain("Method MethodThatReturnsDefaultValue of type MethodValidationInterceptorTestHelper cannot return default value");
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
        public void VoidMethod(int p1, int p2)
        {
        }

        public int MethodThatReturnsNotDefaultValue()
        {
            return 1;
        }

        public int MethodThatReturnsDefaultValue()
        {
            return 0;
        }
    }
}

