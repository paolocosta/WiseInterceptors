using System;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public class AlwaysUsePersistentCacheInvocationManager:PersistentCacheGenericInvocationManager
    {
        public AlwaysUsePersistentCacheInvocationManager(ICache cache, IHelper helper)
            : base(cache, helper)
        {

        }

        protected override bool IsPersistedByDefault
        {
            get { return true; }
        }

        protected override void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate)
        {
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
            InsertValueInPersistentCache(key, value, settings);
        }
    }
}
