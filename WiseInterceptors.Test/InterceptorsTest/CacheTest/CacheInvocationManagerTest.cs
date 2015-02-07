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
        private IInvocation _invocation;
        private CacheInvocationManager _sut;
        private DateTime _time;

        [SetUp]
        public void Setup()
        {
            _cache = Substitute.For<ICache>();
            _helper = Substitute.For<IHelper>();
            _invocation = Substitute.For<IInvocation>();
            _helper.GetCallIdentifier(_invocation).Returns("key");

            _cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, UseCache = true });

            var timeProvider = Substitute.For<TimeProvider>();
            TimeProvider.Current = timeProvider;
            _time = new DateTime(2000, 1, 1);
            TimeProvider.Current.UtcNow.Returns(_time);

            _sut = new CacheInvocationManager(_cache, _helper);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void void_method_when_use_cache_is_true_should_return_invalid_operation_exception()
        {
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(true);
            _helper.GetMethodDescription(Arg.Any<IInvocation>()).Returns(string.Empty);
            
            _sut.GetResult(_invocation);
        }

        [Test]
        public void should_write_in_persistent_cache_when_settings_are_configured_for_default_persistence_and_method_is_called()
        {
            _cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.AlwaysUsePersistentCache, UseCache=true });

            _invocation.ReturnValue.Returns(2);

            _sut.GetResult(_invocation);

            _cache.Received().InsertInPersistantCache("key", 2);
        }

        [Test]
        public void should_write_in_cache_when_method_is_called()
        {
            _invocation.ReturnValue.Returns(2);
            var called = false;
            
            _sut.GetResult(_invocation);
            
            _cache.Received().Insert(Arg.Is<string>("key"), Arg.Is<CacheValue>(x => (int)x.Value == 2), Arg.Any<DateTime>());
        }

        [Test]
        public void should_read_from_persistent_cache_when_settings_are_configured_for_any_kind_of_persistence_and_method_raise_exception()
        {
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
            _cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
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
            _cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.AlwaysUsePersistentCache, UseCache=true });

            _invocation.When(x => x.Proceed()).Do(x => { throw new ApplicationException(); });

            _sut.GetResult(_invocation);
        }

        [Test]
        public void should_write_in_persistent_cache_when_settings_are_configured_for_error_persistence_and_method_raises_exception_and_cache_is_softly_but_not_hardly_expired()
        {
            _cache.Get("key").Returns(new CacheValue { ExpiryDate = _time.AddSeconds(-1), Value = 1 });
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
            _cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError, UseCache=true });

            _invocation.When(x => x.Proceed()).Do(x => { throw new Exception(); });

            _sut.GetResult(_invocation);
            
            _cache.Received().InsertInPersistantCache("key", 1);
        }

        [Test]
        [TestCase(FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError)]
        [TestCase(FaultToleranceEnum.AlwaysUsePersistentCache)]
        public void should_return_softly_expired_value_when_exception_is_raised(FaultToleranceEnum faultTolerance)
        {
            _cache.Get("key").Returns(new CacheValue { ExpiryDate = _time.AddSeconds(-1), Value = 1 });
            _cache.GetFromPersistantCache("key").Returns(2);
            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);
            _cache.GetSettings(Arg.Any<MethodInfo>(), Arg.Any<object[]>())
                .Returns(new CacheSettings { Duration = 20 * 60, Priority = PriorityEnum.Normal, FaultToleranceType = faultTolerance, UseCache = true });

            _invocation.When(x => x.Proceed()).Do(x => { throw new Exception(); });

            var result = _sut.GetResult(_invocation);

            result.Should().Be(1);
        }

        [Test]
        public void when_returned_value_is_softly_expired_insert_in_cache_should_be_called_twice_with_different_expiry_dates()
        {
            var start = TimeProvider.Current.UtcNow;
            var calls = Tuple.Create(0, 0);
            _invocation.ReturnValue.Returns(2);
            _cache.Get(Arg.Any<string>()).Returns(new CacheValue { ExpiryDate = start.AddSeconds(-1), Value = 1 });

            _cache
                .When(x => x.Insert(Arg.Any<string>(), Arg.Is<CacheValue>(y => (int)y.Value == 1), Arg.Is<DateTime>(y => (y - start) > new TimeSpan(20, 0, 0, 0, 0))))
                .Do(x => calls = Tuple.Create(calls.Item1 + 1, calls.Item2));

            _cache
                .When(x => x.Insert(Arg.Any<string>(), Arg.Is<CacheValue>(y => (int)y.Value == 2), Arg.Is<DateTime>(y => (y - start) < new TimeSpan(20, 0, 0, 0, 0))))
                .Do(x => calls = Tuple.Create(calls.Item1, calls.Item2 + 1));

            _helper.IsReturnTypeVoid(Arg.Any<IInvocation>()).Returns(false);

            _sut.GetResult(_invocation);
            
            calls.Should().Be(Tuple.Create(1, 1));
        }

        [Test]
        public void should_return_value_in_cache_if_its_present()
        {
            _invocation.ReturnValue.Returns(2);
            var expiryDate = TimeProvider.Current.UtcNow.AddSeconds(1);
            _cache.Get("key").Returns(new CacheValue { ExpiryDate = expiryDate, Value = 1 });
            
            var result = _sut.GetResult(_invocation);

            result.Should().Be(1);
        }

        [Test]
        public void should_return_real_value_if_object_is_softly_expired()
        {
            _invocation.ReturnValue.Returns(1);
            var expiryDate = TimeProvider.Current.UtcNow.AddSeconds(-1);
            _cache.Get("key").Returns(new CacheValue { Value = 2, ExpiryDate = expiryDate });

            var result = _sut.GetResult(_invocation);

            result.Should().Be(1);
        }

        [TearDown]
        public void TearDown()
        {
            TimeProvider.ResetDefault();
        }
    }
}
