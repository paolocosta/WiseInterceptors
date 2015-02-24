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

        public CacheInvocationStrategySelector(ICache cache, IHelper helper)
        {
            _cache = cache;
            _helper = helper;
        }

        private Dictionary<FaultToleranceEnum, CacheInvocationManager> _cacheInvocationManagerStrategies;
        private Dictionary<FaultToleranceEnum, CacheInvocationManager> CacheInvocationManagerStrategies
        {
            get
            {
                if (_cacheInvocationManagerStrategies == null)
                {
                    _cacheInvocationManagerStrategies = new Dictionary<FaultToleranceEnum, CacheInvocationManager>() 
                    { 
                        {FaultToleranceEnum.AlwaysUsePersistentCache, 
                            new Lazy<AlwaysUsePersistentCacheInvocationManager>
                                (()=> new AlwaysUsePersistentCacheInvocationManager(_cache, _helper)).Value 
                        },
                        {FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors, 
                            new Lazy<ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager>(
                                ()=> new ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager(_cache, _helper)).Value 
                        },
                        {FaultToleranceEnum.FailFastWithNoRecovery, 
                            new Lazy<FailFastCacheInvocationManager>(
                                ()=> new FailFastCacheInvocationManager(_cache, _helper)).Value 
                        },
                        {FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError, 
                            new Lazy<UsePersistentCacheOnlyInCaseOfErrorInvocationManager>(
                                ()=> new UsePersistentCacheOnlyInCaseOfErrorInvocationManager(_cache, _helper)).Value 
                        }
                    };
                }
                return _cacheInvocationManagerStrategies;
            }
        }

        public CacheInvocationManager GetCacheManagerImplementation()
        {            
            var faultToleranceStrategy = _cache.GetFaultToleranceStrategy();
            return CacheInvocationManagerStrategies[faultToleranceStrategy];
        }
    }
}
