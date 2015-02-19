using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using WiseInterceptors.Interceptors.Cache.Strategies;

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
