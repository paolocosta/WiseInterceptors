using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Test.InterceptorsTest.CacheTest
{
    [TestFixture]
    [Category("Cache")]
    public class CacheInterceptorTest
    {
        ///TODO ALL CACHE INVOCATION MANAGERS ARE TO TESTED
        [Test]
        public void should_Intercept_call_build()
        {
            var cache = Substitute.For<ICache>();
            var helper = Substitute.For<IHelper>();
            var invocation = Substitute.For<IInvocation>();
            var invocationManagerStrategySelector = Substitute.For<ICacheInvocationStrategySelector>();
            invocationManagerStrategySelector.GetCacheManagerImplementation().Returns(Substitute.For<CacheInvocationManager>(cache, helper));
            var sut = new CacheInterceptor(cache, helper, invocationManagerStrategySelector);
            sut.Intercept(invocation);

            invocationManagerStrategySelector.Received().GetCacheManagerImplementation();
        }

        [Test]
        public void should_Intercept_call_GetInvocationResult()
        {
            var cache = Substitute.For<ICache>();
            var helper = Substitute.For<IHelper>();
            var invocation = Substitute.For<IInvocation>();
            var invocationManagerStrategySelector = Substitute.For<ICacheInvocationStrategySelector>();
            var cacheInvocationManager = Substitute.For<CacheInvocationManager>(cache, helper);

            invocationManagerStrategySelector.GetCacheManagerImplementation().Returns(cacheInvocationManager);
            var sut = new CacheInterceptor(cache, helper, invocationManagerStrategySelector);
            sut.Intercept(invocation);

            cacheInvocationManager.Received().GetInvocationResult(invocation);
        }
    }
}
