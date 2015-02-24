using Castle.DynamicProxy;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Interceptors.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        readonly ICache _cache;
        readonly IHelper _helper;
        readonly ICacheInvocationStrategySelector _invocationStrategySelector;

        public CacheInterceptor(
            ICache cache, 
            IHelper helper,
            ICacheInvocationStrategySelector invocationStrategySelector)
        {
            _cache = cache;
            _helper = helper;
            _invocationStrategySelector = invocationStrategySelector;
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheInvocationManager = _invocationStrategySelector.GetCacheManagerImplementation();
            invocation.ReturnValue = cacheInvocationManager.GetInvocationResult(invocation);
        }
    }
}
