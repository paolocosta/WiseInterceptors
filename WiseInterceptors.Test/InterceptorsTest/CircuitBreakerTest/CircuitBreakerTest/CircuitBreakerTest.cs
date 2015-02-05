using Autofac;
using Autofac.Extras.DynamicProxy2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Common;
using WiseInterceptor.Interceptors.CircuitBreaker;
using NSubstitute;
using System.Reflection;

namespace WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest
{
    [TestFixture]
    [Category("Circuit Breaker")]
    public class CircuitBreakerTest
    {
        CacheStub _Cache;

        private  IContainer BuildContainer()
        {
            var timeProvider = Substitute.For<TimeProvider>();
            TimeProvider.Current = timeProvider;
            TimeProvider.Current.UtcNow.Returns(DateTime.MinValue);

            var builder = new ContainerBuilder();

            var circuitBreakerSettingsReader = Substitute.For<ICircuitBreakerSettingsReader>();
            
            circuitBreakerSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(new CircuitBreakerSettings
            {
                BreakingPeriodInSeconds = 30,
                ExceptionsBeforeBreak = 3,
                ExceptionType = typeof(Exception),
                RetryingPeriodInSeconds = 30
            });
            
            builder.RegisterModule<InterceptorModule>();
            builder.RegisterType<CacheStub>().As<ICache>();
            builder.Register(c => circuitBreakerSettingsReader).As<ICircuitBreakerSettingsReader>();
            builder.RegisterType<Breakable>()
            .EnableClassInterceptors()
            .InterceptedBy(typeof(CircuitBreakerInterceptor));

            var container = builder.Build();
            return container;

            
        }

        [Test]
        [Category("Circuit Breaker")]
        public void call_with_no_exception_should_not_raise_exception()
        {
            var container = BuildContainer();

            var breakable = container.Resolve<Breakable>();

            breakable.Call(false);

            Assert.IsTrue(true);
        }

        [Test]
        [Category("Circuit Breaker")]
        [ExpectedException(typeof(TimeoutException))]
        public void call_with_exception_should_raise_exception()
        {
            var container = BuildContainer();

            var breakable = container.Resolve<Breakable>();

            breakable.Call(true);
        }

        [Test]
        [Category("Circuit Breaker")]
        public void three_calls_with_exception_should_raise_three_timeout_exception()
        {
            var container = BuildContainer();
            List<Exception> exceptions = new List<Exception>();
            var breakable = container.Resolve<Breakable>();

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    breakable.Call(true);
                }
                catch (TimeoutException ex)
                {
                    exceptions.Add(ex);
                }
                TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddSeconds(1));
            }
            Assert.IsTrue(exceptions.Where(x => x.GetType() == typeof(TimeoutException)).Count() == 3);
        }

        [Test]
        [Category("Circuit Breaker")]
        public void three_calls_with_exception_and_one_with_no_exception_in_less_than_60_seconds_should_raise_three_timeout_exception_and_one_circuit_breaker_exception()
        {
            var container = BuildContainer();
            Dictionary<int, Exception> exceptions = new Dictionary<int, Exception>();
            var breakable = container.Resolve<Breakable>();

            for (int i = 0; i < 4; i++)
            {
                try
                {

                    breakable.Call(i<3?true:false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(i, ex);
                }
                TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddSeconds(1));
            }

            Assert.IsTrue(
                    exceptions.Count() == 4 
                &&  exceptions.Where(x => x.Value.GetType() == typeof(TimeoutException)).Count() == 3
                &&  exceptions[3].GetType() == typeof(CircuitBreakerException)
                );
        }

        [Test]
        [Category("Circuit Breaker")]
        public void four_calls_with_exception_in_more_than_60_seconds_should_raise_four_timeout_exception()
        {
            var container = BuildContainer();
            Dictionary<int, Exception> exceptions = new Dictionary<int, Exception>();
            var breakable = container.Resolve<Breakable>();

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    breakable.Call(true);
                }
                catch (Exception ex)
                {
                    exceptions.Add(i, ex);
                }
                if(i < 2)
                    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddSeconds(1));
                else
                    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(2));
            }

            Assert.IsTrue(
                    exceptions.Count() == 4
                && exceptions.Where(x => x.Value.GetType() == typeof(TimeoutException)).Count() == 4
                );
        }

        [Test]
        [Category("Circuit Breaker")]
        public void four_calls_with_exception_in_less_than_60_seconds_should_raise_three_timeout_exceptions_and_one_circuitbreaker_exception_next_call_after_more_than_60_seconds_should_pass()
        {
            var container = BuildContainer();
            Dictionary<int, Exception> exceptions = new Dictionary<int, Exception>();
            var breakable = container.Resolve<Breakable>();

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    breakable.Call(i < 4 ? true : false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(i, ex);
                }
                if (i < 3)
                    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddSeconds(1));
                else
                    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(2));
            }

            Assert.IsTrue(
                    exceptions.Count() == 4
                && exceptions.Where(x => x.Value.GetType() == typeof(TimeoutException)).Count() == 3
                && exceptions[3].GetType() == typeof(CircuitBreakerException)
                );
        }
    }

    public class Breakable 
    {
        public virtual void Call(bool raiseException)
        {
            if(raiseException)
                throw new TimeoutException();
        }
    }

   
}
