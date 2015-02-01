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
using Castle.DynamicProxy;
using WiseInterceptor.Common;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    [TestFixture]
    [Category("Cache")]
    public class CacheTest
    {
        CacheStub _Cache;
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            var timeProvider = Substitute.For<TimeProvider>();
            TimeProvider.Current = timeProvider;
            TimeProvider.Current.UtcNow.Returns(DateTime.MinValue);
            
            
            builder.RegisterModule<InterceptorModule>();
            builder.RegisterType<CacheStub>().As<ICache>();
            //builder.Register(c => new CacheInterceptor(_Cache));

            builder.RegisterType<Cachable>()
            .EnableClassInterceptors()
            .InterceptedBy(typeof(CacheInterceptor));

            _container = builder.Build();
        }

        [Test]
        public void two_near_identical_subsequent_call_should_return_the_same_result_even_if_the_value_changes()
        {
            var cachable = _container.Resolve<Cachable>();
            var name = Tuple.Create("Stale","Actual");
            var lastName = "Last Name";
            
            cachable.SetName(name.Item1);
            var firstResult = cachable.Hello(lastName);
            cachable.SetName(name.Item2);
            TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(1));
            var secondResult = cachable.Hello(lastName);
            
            firstResult.Should().Be(secondResult);
        }

        [Test]
        public void two_distant_identical_subsequent_call_should_return_different_results_if_the_value_changes()
        {
            var cachable = _container.Resolve<Cachable>();
            var name = Tuple.Create("Stale", "Actual");
            var lastName = "Last Name";

            cachable.SetName(name.Item1);
            var firstResult = cachable.Hello(lastName);
            cachable.SetName(name.Item2);
            TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(21));
            //TimeProvider.Current.SetCurrentTime(TimeProvider.Current.UtcNow.AddMinutes(21));
            var secondResult = cachable.Hello(lastName);

            firstResult.Should().NotBe(secondResult);
        }
       
        [Test]
        public void two_distinct_subsequent_call_should_return_different_results_if_the_the_method_args_are_different_and_of_course_the_query_result_changes()
        {
            var cachable = _container.Resolve<Cachable>();
            var name = Tuple.Create("Stale", "Actual");
            var args = Tuple.Create("args1", "args2");

            cachable.SetName(name.Item1);
            var firstResult = cachable.Hello(args.Item1);
            cachable.SetName(name.Item2);
            TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(1));
            //TimeProvider.Current.SetCurrentTime(TimeProvider.Current.UtcNow.AddMinutes(1));
            var secondResult = cachable.Hello(args.Item2);

            firstResult.Should().NotBe(secondResult);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void void_method_decorated_with_cache_should_return_exception()
        {
            var cachable = _container.Resolve<Cachable>();
            
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
            TimeProvider.Current.UtcNow.Returns(start);
            //TimeProvider.Current.SetCurrentTime(start);
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

            var interceptor = new CacheInterceptor(cache, helper);
            
            interceptor.Intercept(invocation);

            //we checked that both calls where performed correctly
            calls.Should().Be(Tuple.Create(1, 1));
        }
    }

    public class Cachable
    {
        public string Name { get; set; }

        [CacheSettings]

        public virtual string Hello(string arg)
        {
            return Name;
        }

        //This nonsense method is intended to test the unhappy case
        [CacheSettings]
        public virtual void DoNothing()
        {
            
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }

    
}
