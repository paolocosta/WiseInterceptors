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
        public void two_near_subsequent_call_should_return_stale_result_even_if_the_value_changes()
        {
            var container = BuildContainer();

            var cachable = container.Resolve<ICachable>();

            var name = Tuple.Create("Stale","Actual");

            cachable.SetName(name.Item1);

            cachable.GetName();

            cachable.SetName(name.Item2);

            _Cache.FakeNow = _Cache.FakeNow.AddMinutes(1);

            var result = cachable.GetName();

            result.Should().Be(name.Item1);
        }

        [Test]
        public void two_distant_subsequent_call_should_return_different_result_if_the_value_changes()
        {
            var container = BuildContainer();

            var cachable = container.Resolve<ICachable>();

            var name = Tuple.Create("Stale", "Actual");

            cachable.SetName(name.Item1);

            cachable.GetName();

            cachable.SetName(name.Item2);

            _Cache.FakeNow = _Cache.FakeNow.AddMinutes(21);

            var result = cachable.GetName();

            result.Should().Be(name.Item2);
        }

    }

    public class Cachable:ICachable
    {
        public string Name { get; set; }

        [CacheSettings]
        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }

    public interface ICachable
    {
        string GetName();
        void SetName(string name);
    }
}
