using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public class AlwaysUsePersistentCacheInvocationManager:PersistentCacheGenericInvocationManager
    {
        public AlwaysUsePersistentCacheInvocationManager(ICache cache, IHelper helper, CacheSettings settings)
            : base(cache, helper, settings)
        {

        }

        protected override bool IsPersistedByDefault()
        {
            return true;
        }

        protected override void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate)
        {
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
            InsertValueInPersistentCache(key, value, settings);
        }
    }
}
