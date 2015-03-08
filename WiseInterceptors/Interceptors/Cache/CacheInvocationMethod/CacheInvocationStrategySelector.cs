using System;
using System.Collections.Generic;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.Strategies;

namespace WiseInterceptors.Interceptors.Cache.CacheInvocationMethod
{
    public interface ICacheInvocationStrategySelector
    {
        CacheInvocationManager GetCacheManagerImplementation();
    }

    public class CacheInvocationStrategySelector : ICacheInvocationStrategySelector
    {
        readonly ICache _cache;
        readonly IHelper _helper;
        readonly Func<FaultToleranceEnum, CacheInvocationManager> _cacheInvocationManagerFactory;


        public CacheInvocationStrategySelector(
            ICache cache, 
            IHelper helper,
            Func<FaultToleranceEnum, CacheInvocationManager> cacheInvocationManagerFactory)
        {
            _cache = cache;
            _helper = helper;
            _cacheInvocationManagerFactory = cacheInvocationManagerFactory;
        }

        public CacheInvocationManager GetCacheManagerImplementation()
        {
            return _cacheInvocationManagerFactory(_cache.GetFaultToleranceStrategy());
        }
    }
}
