using Autofac;
using Autofac.Extras.DynamicProxy2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.CircuitBreaker;


namespace WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest
{
    [TestFixture]
    public class CircuitBreakerTest
    {
        CacheStub _Cache;

        private  IContainer BuildContainer()
        {
            
            var builder = new ContainerBuilder();

            _Cache = new CacheStub();

            _Cache.FakeNow = DateTime.MinValue;
           
            builder.Register(c => new CircuitBreakerInterceptor(_Cache));

            builder.RegisterType<Breakable>()
                .As<IBreakable>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(CircuitBreakerInterceptor));

            var container = builder.Build();
            return container;
        }

        [Test]
        public void call_with_no_exception_should_not_raise_exception()
        {
            var container = BuildContainer();

            var breakable = container.Resolve<IBreakable>();

            breakable.Call(false);

            Assert.IsTrue(true);
        }

        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public void call_with_exception_should_raise_exception()
        {
            var container = BuildContainer();

            var breakable = container.Resolve<IBreakable>();

            breakable.Call(true);
        }

        [Test]
        public void three_calls_with_exception_should_raise_three_timeout_exception()
        {
            var container = BuildContainer();
            List<Exception> exceptions = new List<Exception>();
            var breakable = container.Resolve<IBreakable>();

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
                _Cache.FakeNow = _Cache.FakeNow.AddSeconds(1);
            }
            Assert.IsTrue(exceptions.Where(x => x.GetType() == typeof(TimeoutException)).Count() == 3);
        }

        [Test]
        public void three_calls_with_exception_and_one_with_no_exception_in_less_than_60_seconds_should_raise_three_timeout_exception_and_one_circuit_breaker_exception()
        {
            var container = BuildContainer();
            Dictionary<int, Exception> exceptions = new Dictionary<int, Exception>();
            var breakable = container.Resolve<IBreakable>();

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
                _Cache.FakeNow = _Cache.FakeNow.AddSeconds(1);
            }

            Assert.IsTrue(
                    exceptions.Count() == 4 
                &&  exceptions.Where(x => x.Value.GetType() == typeof(TimeoutException)).Count() == 3
                &&  exceptions[3].GetType() == typeof(CircuitBreakerException)
                );
        }

        [Test]
        public void four_calls_with_exception_in_more_than_60_seconds_should_raise_four_timeout_exception()
        {
            var container = BuildContainer();
            Dictionary<int, Exception> exceptions = new Dictionary<int, Exception>();
            var breakable = container.Resolve<IBreakable>();

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
                    _Cache.FakeNow = _Cache.FakeNow.AddSeconds(1);
                else
                    _Cache.FakeNow = _Cache.FakeNow.AddMinutes(2);
            }

            Assert.IsTrue(
                    exceptions.Count() == 4
                && exceptions.Where(x => x.Value.GetType() == typeof(TimeoutException)).Count() == 4
                );
        }

        [Test]
        public void four_calls_with_exception_in_less_than_60_seconds_should_raise_three_timeout_exceptions_and_one_circuitbreaker_exception_next_call_after_more_than_60_seconds_should_pass()
        {
            var container = BuildContainer();
            Dictionary<int, Exception> exceptions = new Dictionary<int, Exception>();
            var breakable = container.Resolve<IBreakable>();

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
                    _Cache.FakeNow = _Cache.FakeNow.AddSeconds(1);
                else
                    _Cache.FakeNow = _Cache.FakeNow.AddMinutes(2);
            }

            Assert.IsTrue(
                    exceptions.Count() == 4
                && exceptions.Where(x => x.Value.GetType() == typeof(TimeoutException)).Count() == 3
                && exceptions[3].GetType() == typeof(CircuitBreakerException)
                );
        }
    }

    public class Breakable : WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest.IBreakable 
    {
        [CircuitBreakerSettings(
            ExceptionType = typeof(Exception), 
            RetryingPeriodInSeconds = 60, 
            BreakingPeriodInSeconds = 60, 
            ExceptionsBeforeBreak = 3)]
        public virtual void Call(bool raiseException)
        {
            if(raiseException)
                throw new TimeoutException();
        }
    }

    public interface IBreakable
    {
        void Call(bool raiseException);
    }
}
