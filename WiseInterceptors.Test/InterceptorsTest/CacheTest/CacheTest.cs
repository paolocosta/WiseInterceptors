using Autofac;
using Autofac.Extras.DynamicProxy2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Cache;
using WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest;
using FluentAssertions;
using WiseInterceptor;
using NSubstitute;
using WiseInterceptor.Interceptors.Common;
using Castle.DynamicProxy;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    [TestFixture]
    public class CacheTest
    {
        CacheStub _Cache;

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            _Cache = new CacheStub();

            _Cache.FakeNow = DateTime.MinValue;

            builder.Register(c => new CacheInterceptor(_Cache));

            builder.RegisterType<Cachable>().As<ICachable>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(CacheInterceptor));

            var container = builder.Build();
            return container;
        }

        [Test]
        public void two_near_identical_subsequent_call_should_return_the_same_result_even_if_the_value_changes()
        {
            var container = BuildContainer();
            var cachable = container.Resolve<ICachable>();
            var name = Tuple.Create("Stale","Actual");
            var lastName = "Last Name";
            
            cachable.SetName(name.Item1);
            var firstResult = cachable.Hello(lastName);
            cachable.SetName(name.Item2);
            _Cache.FakeNow = _Cache.FakeNow.AddMinutes(1);
            var secondResult = cachable.Hello(lastName);
            
            firstResult.Should().Be(secondResult);
        }

        [Test]
        public void two_distant_identical_subsequent_call_should_return_different_results_if_the_value_changes()
        {
            var container = BuildContainer();
            var cachable = container.Resolve<ICachable>();
            var name = Tuple.Create("Stale", "Actual");
            var lastName = "Last Name";

            cachable.SetName(name.Item1);
            var firstResult = cachable.Hello(lastName);
            cachable.SetName(name.Item2);
            _Cache.FakeNow = _Cache.FakeNow.AddMinutes(21);
            var secondResult = cachable.Hello(lastName);

            firstResult.Should().NotBe(secondResult);
        }

        [Test]
        public void two_distinct_subsequent_call_should_return_different_results_if_the_value_is_the_same()
        {
            var container = BuildContainer();
            var cachable = container.Resolve<ICachable>();
            var name = Tuple.Create("Stale", "Actual");
            var lastName = Tuple.Create("Stale last name", "Actual last name");

            cachable.SetName(name.Item1);
            var firstResult = cachable.Hello(lastName.Item1);
            cachable.SetName(name.Item2);
            _Cache.FakeNow = _Cache.FakeNow.AddMinutes(1);
            var secondResult = cachable.Hello(lastName.Item2);

            firstResult.Should().NotBe(secondResult);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void void_method_decorated_with_cache_should_return_exception()
        {
            var container = BuildContainer();
            
            var cachable = container.Resolve<ICachable>();
            
            cachable.DoNothing();
        }

        [Test]
        public void when_returned_value_is_softly_expired_insert_in_cache_should_be_called_twice_with_different_expiry_dates()
        {
            var start = new DateTime(2000, 1, 1);
            var calls = Tuple.Create(0, 0);

            var invocation = Substitute.For<IInvocation>();

            //new value returned from method equals 2
            invocation.ReturnValue.Returns(2);

            var cache = NSubstitute.Substitute.For<ICache>();
            cache.Now().Returns(start);
            //Value returned from cache is sotly expired and equals 1
            cache.Get(Arg.Any<string>()).Returns(new CacheValue { ExpiryDate = start.AddSeconds(-1), Value = 1 });

            //first call to _Cache.Insert has a long expiration and value equals 1 as the value returned from cache
            cache
                .When(x => x.Insert(Arg.Any<string>(), Arg.Is<CacheValue>(y => (int)y.Value == 1), Arg.Is<DateTime>(y => (y - start) > new TimeSpan(20, 0, 0, 0, 0))))
                .Do(x => calls = Tuple.Create(calls.Item1 + 1, calls.Item2));

            //second call to _Cache.Insert has a short expiration and value equals to
            cache
                .When(x => x.Insert(Arg.Any<string>(), Arg.Is<CacheValue>(y => (int)y.Value == 2), Arg.Is<DateTime>(y => (y - start) < new TimeSpan(20, 0, 0, 0, 0))))
                .Do(x => calls = Tuple.Create(calls.Item1, calls.Item2 + 1));

            var helper = Substitute.For<IHelper>();
            helper.GetInvocationMethodAttribute<CacheSettingsAttribute>(Arg.Any<IInvocation>())
                .Returns(new CacheSettingsAttribute { Duration = 20 * 60, Priority = PriorityEnum.Normal });
            helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);

            var interceptor = new CacheInterceptor(cache);
            interceptor.SetHelper(helper);
            
            interceptor.Intercept(invocation);

            //we checked that both calls where performed correctly
            calls.Should().Be(Tuple.Create(1, 1));
        }
    }

    public class Cachable:ICachable
    {
        public string Name { get; set; }

        [CacheSettings]
        public string Hello(string LastName)
        {
            return string.Format("{0} {1}",  Name, LastName);
        }

        [CacheSettings]
        public void DoNothing()
        {
            
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }

    public interface ICachable
    {
        string Hello(string LastName);
        void SetName(string name);
        void DoNothing();
    }
}
