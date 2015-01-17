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
        public void do_void_should_return_exception()
        {
            var container = BuildContainer();
            
            var cachable = container.Resolve<ICachable>();
            
            cachable.DoNothing();
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
