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
}
