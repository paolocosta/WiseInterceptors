using System;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public class ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager : CacheInvocationManager
    {
        public ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager(ICache cache, IHelper helper)
            : base(cache, helper)
        {
            
        }

        protected override bool IsPersistedByDefault
        {
            get { return false; }
        }

        protected override object HandleInvocationException(CacheSettings settings, string key, CacheValue valueFromCache, CacheMethodInvocationException ex)
        {
            if (valueFromCache == null)
            {
                throw ex.InnerException;
            }
            else
            {
                return valueFromCache.Value;
            }
        }

        protected override void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate)
        {
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
        }
    }
}

