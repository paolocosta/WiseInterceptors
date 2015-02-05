using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Common;
using WiseInterceptor.Interceptors.Cache;
using FluentAssertions;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    public class CacheInvocationManagerTest
    {
        private ICache _cache;
        private IHelper _helper;
        private ICacheSettingsReader _cacheSettingsReader;
        private IInvocation _invocation;
        private CacheInvocationManager _sut;
        private DateTime _time;

        [SetUp]
        public void Setup()
        {
            _cache = Substitute.For<ICache>();
            _helper = Substitute.For<IHelper>();
            _cacheSettingsReader = Substitute.For<ICacheSettingsReader>();
            _invocation = Substitute.For<IInvocation>();

            var timeProvider = Substitute.For<TimeProvider>();
            TimeProvider.Current = timeProvider;
            _time = new DateTime(2000, 1, 1);
            TimeProvider.Current.UtcNow.Returns(_time);

            _sut = new CacheInvocationManager(_cache, _helper, _cacheSettingsReader);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void void_method_when_use_cache_is_true_should_return_invalid_operation_exception()
        {
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(true);
            _helper.GetMethodDescription(Arg.Any<IInvocation>()).Returns(string.Empty);
            
            _cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>()).Returns(
                new CacheSettings{ UseCache=true, Duration=20, FaultToleranceType=FaultToleranceEnum.AlwaysUsePersistentCache});
            
            _sut.GetResult(_invocation);
        }

        [Test]
        public void should_write_in_persistent_cache_when_settings_are_configured_for_default_persistence_and_method_is_called()
        {
            _cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.AlwaysUsePersistentCache, UseCache=true });

            _helper.GetCallIdentifier(_invocation).Returns("key");
            _invocation.ReturnValue.Returns(2);

            _sut.GetResult(_invocation);

            _cache.Received().InsertInPersistantCache("key", 2);
        }

        [Test]
        public void should_read_from_persistent_cache_when_settings_are_configured_for_any_kind_of_persistence_and_method_raise_exception()
        {
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
            _helper.GetCallIdentifier(_invocation).Returns("key");
            _cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.AlwaysUsePersistentCache, UseCache=true });
            _cache.GetFromPersistantCache("key").Returns(1);
            _invocation.When(x => x.Proceed()).Do(x => { throw new Exception(); });

            var result = _sut.GetResult(_invocation);

            result.Should().Be(1);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void should_throw_the_same_exception_when_settings_are_configured_for_any_kind_of_persistence_and_method_raise_exception_and_object_is_not_in_cache_nor_in_persistent_cache()
        {
            _cache.GetFromPersistantCache("key").Returns(null);
            _cache.Get("key").Returns(null);
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
            _helper.GetCallIdentifier(_invocation).Returns("key");
            _cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.AlwaysUsePersistentCache, UseCache=true });

            _invocation.When(x => x.Proceed()).Do(x => { throw new ApplicationException(); });

            _sut.GetResult(_invocation);
        }

        [Test]
        public void should_write_in_persistent_cache_when_settings_are_configured_for_error_persistence_and_method_raise_exception_and_cache_is_softly_but_not_hardly_expired()
        {
            _cache.Get("key").Returns(new CacheValue { ExpiryDate = _time.AddSeconds(-1), Value = 1 });
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
            _helper.GetCallIdentifier(_invocation).Returns("key");
            _cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError, UseCache=true });

            _invocation.When(x => x.Proceed()).Do(x => { throw new Exception(); });

            _sut.GetResult(_invocation);
            
            _cache.Received().InsertInPersistantCache("key", 1);
        }

        //[Test]
        //public void should_return_softly_expired_value_when_exception_is_raised()
        //{
        //    var time = new DateTime(2000, 1, 1);
        //    TimeProvider.Current.UtcNow.Returns(time);
        //    var cache = NSubstitute.Substitute.For<ICache>();
        //    cache.Get("key").Returns(new CacheValue { ExpiryDate = time.AddSeconds(-1), Value = 1 });
        //    cache.GetFromPersistantCache("key").Returns(2);
        //    var helper = Substitute.For<IHelper>();
        //    helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
        //    var invocation = Substitute.For<IInvocation>();
        //    helper.GetCallIdentifier(invocation).Returns("key");
        //    var cacheSettingsReader = Substitute.For<ICacheSettingsReader>();
        //    cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
        //        .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.AlwaysUsePersistentCache });

        //    invocation.When(x => x.Proceed()).Do(x => { throw new Exception(); });

        //    var interceptor = new CacheInterceptor(cache, helper, cacheSettingsReader, invocationManagerFactory);

        //    interceptor.Intercept(invocation);

        //    invocation.ReturnValue.Should().Be(1);
        //}

        //[Test]
        //public void when_returned_value_is_softly_expired_insert_in_cache_should_be_called_twice_with_different_expiry_dates()
        //{
        //    var start = new DateTime(2000, 1, 1);
        //    var calls = Tuple.Create(0, 0);

        //    var invocation = Substitute.For<IInvocation>();

        //    //new value returned from method equals 2
        //    invocation.ReturnValue.Returns(2);

        //    var cache = NSubstitute.Substitute.For<ICache>();
        //    TimeProvider.Current.UtcNow.Returns(start);
        //    //TimeProvider.Current.SetCurrentTime(start);
        //    //Value returned from cache is sotly expired and equals 1
        //    cache.Get(Arg.Any<string>()).Returns(new CacheValue { ExpiryDate = start.AddSeconds(-1), Value = 1 });

        //    //first call to _Cache.Insert has a long expiration and value equals 1 as the value returned from cache
        //    cache
        //        .When(x => x.Insert(Arg.Any<string>(), Arg.Is<CacheValue>(y => (int)y.Value == 1), Arg.Is<DateTime>(y => (y - start) > new TimeSpan(20, 0, 0, 0, 0))))
        //        .Do(x => calls = Tuple.Create(calls.Item1 + 1, calls.Item2));

        //    //second call to _Cache.Insert has a short expiration and value equals to
        //    cache
        //        .When(x => x.Insert(Arg.Any<string>(), Arg.Is<CacheValue>(y => (int)y.Value == 2), Arg.Is<DateTime>(y => (y - start) < new TimeSpan(20, 0, 0, 0, 0))))
        //        .Do(x => calls = Tuple.Create(calls.Item1, calls.Item2 + 1));

        //    var helper = Substitute.For<IHelper>();
        //    helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);

        //    var cacheSettingsReader = Substitute.For<ICacheSettingsReader>();
        //    cacheSettingsReader.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
        //        .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal });

        //    var interceptor = new CacheInterceptor(cache, helper, cacheSettingsReader, invocationManagerFactory);

        //    interceptor.Intercept(invocation);

        //    //we checked that both calls where performed correctly
        //    calls.Should().Be(Tuple.Create(1, 1));
        //}

        //[Test]
        //public void two_near_identical_subsequent_call_should_return_the_same_result_even_if_the_value_changes()
        //{
        //    var cachable = _container.Resolve<Cachable>();
        //    var name = Tuple.Create("Stale", "Actual");
        //    var lastName = "Last Name";

        //    cachable.SetName(name.Item1);
        //    var firstResult = cachable.Hello(lastName);
        //    cachable.SetName(name.Item2);
        //    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(1));
        //    var secondResult = cachable.Hello(lastName);

        //    firstResult.Should().Be(secondResult);
        //}

        //[Test]
        //public void two_distant_identical_subsequent_call_should_return_different_results_if_the_value_changes()
        //{
        //    var cachable = _container.Resolve<Cachable>();
        //    var name = Tuple.Create("Stale", "Actual");
        //    var lastName = "Last Name";

        //    cachable.SetName(name.Item1);
        //    var firstResult = cachable.Hello(lastName);
        //    cachable.SetName(name.Item2);
        //    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(21));
        //    //TimeProvider.Current.SetCurrentTime(TimeProvider.Current.UtcNow.AddMinutes(21));
        //    var secondResult = cachable.Hello(lastName);

        //    firstResult.Should().NotBe(secondResult);
        //}

        //[Test]
        //public void two_distinct_subsequent_call_should_return_different_results_if_the_the_method_args_are_different_and_of_course_the_query_result_changes()
        //{
        //    var cachable = _container.Resolve<Cachable>();
        //    var name = Tuple.Create("Stale", "Actual");
        //    var args = Tuple.Create("args1", "args2");

        //    cachable.SetName(name.Item1);
        //    var firstResult = cachable.Hello(args.Item1);
        //    cachable.SetName(name.Item2);
        //    TimeProvider.Current.UtcNow.Returns(TimeProvider.Current.UtcNow.AddMinutes(1));
        //    //TimeProvider.Current.SetCurrentTime(TimeProvider.Current.UtcNow.AddMinutes(1));
        //    var secondResult = cachable.Hello(args.Item2);

        //    firstResult.Should().NotBe(secondResult);
        //}
    }
}
