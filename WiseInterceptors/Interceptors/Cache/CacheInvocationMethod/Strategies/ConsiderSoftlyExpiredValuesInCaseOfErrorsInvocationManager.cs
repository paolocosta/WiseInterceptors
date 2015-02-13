using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public class ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager : CacheInvocationManager
    {
        public ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager(ICache cache, IHelper helper, CacheSettings settings)
            : base(cache, helper, settings)
        {
            
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

        protected override bool IsPersistedByDefault()
        {
            return false;
        }

        protected override void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate)
        {
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
        }
    }
}

