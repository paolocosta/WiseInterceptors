using System;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public class FailFastCacheInvocationManager:CacheInvocationManager
    {
        public FailFastCacheInvocationManager(ICache cache, IHelper helper)
            : base(cache, helper)
        {
            
        }

        protected override bool IsPersistedByDefault
        {
            get { return false; }
        }

        protected override object HandleInvocationException(CacheSettings settings, string key, CacheValue valueFromCache, CacheMethodInvocationException ex)
        {
            _cache.Remove(key);
            throw ex.InnerException;
        }

        protected override void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate)
        {
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
        }
    }
}
