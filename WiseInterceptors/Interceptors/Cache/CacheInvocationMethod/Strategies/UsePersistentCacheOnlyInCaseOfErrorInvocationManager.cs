using System;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public class UsePersistentCacheOnlyInCaseOfErrorInvocationManager:PersistentCacheGenericInvocationManager
    {
        public UsePersistentCacheOnlyInCaseOfErrorInvocationManager(ICache cache, IHelper helper)
            : base(cache, helper)
        {

        }

        protected override bool IsPersistedByDefault
        {
            get { return false; }
        }

        protected override void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate)
        {
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
        }
    }
}
