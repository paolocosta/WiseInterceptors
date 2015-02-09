using Castle.DynamicProxy;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Interceptors.Logger;
using NUnit.Framework;
using System.Reflection;
using System.Threading;

namespace WiseInterceptors.Test.InterceptorsTest.LoggerTest
{
    public class LoggerInterceptorTest
    {
        LoggerInterceptor _sut;
        IInvocation _invocation;
        ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _invocation = Substitute.For<IInvocation>();
            _logger = Substitute.For<ILogger>();
            _sut = new LoggerInterceptor(_logger);
            _logger.GetLogSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new LogSettings { Logcall = false });
        }

        [Test]
        public void should_call_proceed()
        {
            _sut.Intercept(_invocation);
            _invocation.Received().Proceed();
        }

        [Test]
        [TestCase(true, 1,2)]
        [TestCase(false, 3, 4)]
        public void should_log_current_method_if_configured_to_log(bool throwException, int parameter1, int parameter2)
        {
            _logger.GetLogSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new LogSettings  { Logcall=true,  LogException = throwException });
            object[] parameters = new object[] { parameter1, parameter2 };
            _invocation.MethodInvocationTarget.Returns(MethodInfo.GetCurrentMethod());
            _invocation.Arguments.Returns(parameters);
            string currentMethodName = MethodInfo.GetCurrentMethod().Name;

            if (throwException)
                _invocation.When(x => x.Proceed()).Do(x => { throw new ApplicationException(); });
            
            try
            {
                _sut.Intercept(_invocation);
            }
            catch (ApplicationException ex)
            {
                if (!throwException)
                    throw ex;
            }
            _logger.Received().Log(Arg.Is<LogInformation>(x => x.Method.Name == currentMethodName && x.Parameters == parameters));
        }

        [Test]
        public void should_not_call_log_if_not_configured_to_log()
        {
            _logger.GetLogSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new LogSettings { Logcall = false });
            
            _sut.Intercept(_invocation);
            
            _logger.DidNotReceive().Log(Arg.Any<LogInformation>());
        }

        [Test]
        public void should_not_call_log_if_configured_to_log_exception_and_no_exception_is_raised()
        {
            _logger.GetLogSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new LogSettings { Logcall = false, LogException = true });
            
            _sut.Intercept(_invocation);
            
            _logger.DidNotReceive().Log(Arg.Any<LogInformation>());
        }

        [Test]
        public void should_logger_return_exception_if_configured_to_log_exception_and_exception_is_raised()
        {
            _logger.GetLogSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new LogSettings { Logcall = false, LogException = true });
            _invocation.When(x => x.Proceed()).Do(x => { throw new ApplicationException(); });

            try
            {
                _sut.Intercept(_invocation);
            }
            catch (ApplicationException)
            { }

            _logger.Received().Log(Arg.Is<LogInformation>(x => x.RaisedException.GetType() == typeof(ApplicationException)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void should_log_execution_time(bool throwException)
        {
            _logger.GetLogSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new LogSettings { Logcall = true, LogException = throwException });
            if(throwException)
                _invocation.When(x => x.Proceed()).Do(x => {Thread.Sleep(1);  throw new ApplicationException(); });
            else
                _invocation.When(x => x.Proceed()).Do(x => { Thread.Sleep(1); });
            try
            {
                _sut.Intercept(_invocation);
            }
            catch (ApplicationException ex)
            {
                if (!throwException)
                    throw ex;
            }
            _logger.Received().Log(Arg.Is<LogInformation>(x => x.ExecutionTime > 0));
        }

    }
}
