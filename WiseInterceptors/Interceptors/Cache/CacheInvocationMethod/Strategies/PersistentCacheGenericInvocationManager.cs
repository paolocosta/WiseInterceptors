﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;

namespace WiseInterceptors.Interceptors.Cache.Strategies
{
    public abstract class PersistentCacheGenericInvocationManager : CacheInvocationManager
    {
        public PersistentCacheGenericInvocationManager(ICache cache, IHelper helper, CacheSettings settings)
            : base(cache, helper, settings)
        {

        }

        protected override object HandleInvocationException(CacheSettings settings, string key, CacheValue valueFromCache, CacheMethodInvocationException ex)
        {
            if (valueFromCache != null)
            {
                if (!valueFromCache.Persisted)
                {
                    _cache.InsertInPersistentCache(key, valueFromCache.Value);
                    CalculateExpirationsAndAddValueToVolatileCache(key, valueFromCache.Value, settings, true);
                }
                return valueFromCache.Value;
            }

            object valueFromPersistentCache = _cache.GetFromPersistentCache(key);

            if (valueFromPersistentCache != null)
            {
                CalculateExpirationsAndAddValueToVolatileCache(key, valueFromPersistentCache, settings, true);
                return valueFromPersistentCache;
            }

            throw ex.InnerException;
        }
    }
}