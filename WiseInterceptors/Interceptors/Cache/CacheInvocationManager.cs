using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.Cache
{
    public interface ICacheInvocationManager
    {
        object GetInvocationResult(IInvocation invocation); 
    }

    public class CacheInvocationManager : ICacheInvocationManager
    {
        readonly ICache _cache;
        readonly IHelper _helper;
        
        public CacheInvocationManager(ICache cache, IHelper helper)
        {
            _cache = cache;
            _helper = helper;
        }

        private void FailIfInterceptedMethodReturnsVoid(IInvocation invocation)
        {
            if (_helper.IsReturnTypeVoid(invocation))
            {
                throw new InvalidOperationException(string.Format("Cache interceptor was added to method {0} but it is not allowed as it returns void",
                    _helper.GetMethodDescription(invocation)
                    ));
            }
        }

        public object GetInvocationResult(IInvocation invocation)
        {
            this.FailIfInterceptedMethodReturnsVoid(invocation);

            CacheSettings settings = _cache.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);

            if (!settings.UseCache)
            {
                return GetActualResult(invocation);
            }
            else
            {
                var key = GetInvocationKey(invocation, settings);
                
                var valueFromCache = GetValueFromVolatileCache(key);

                if (valueFromCache != null)
                {
                    if (IsValueFromVolatileCacheValid(valueFromCache))
                    {
                        return valueFromCache.Value;
                    }
                    else
                    {
                        IncreaseSoftExpirationDateWhileQueryIsPerformed(key, valueFromCache, false);
                    }
                }

                lock (string.Intern(key))
                {
                    try
                    {
                        return DoRealCall(invocation, settings, key);
                    }
                    catch (CacheMethodInvocationException ex)
                    {
                        return HandleInvocationException(settings, key, valueFromCache, ex);
                    }
                }
            }
        }

        private object DoRealCall(IInvocation invocation, CacheSettings settings, string key)
        {
            var result = GetActualResult(invocation);
            CalculateExpirationsAndAddValueToVolatileCache(key, result, settings, settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache);
            InsertValueInPersistentCache(key, result, settings);
            return result;
        }

        private object HandleInvocationException(CacheSettings settings, string key, CacheValue valueFromCache, CacheMethodInvocationException ex)
        {
            if (settings.FaultToleranceType == FaultToleranceEnum.FailFastWithNoRecovery)
            {
                _cache.Remove(key);
                throw ex.InnerException;
            }

            if (valueFromCache == null && settings.FaultToleranceType == FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors)
            {
                throw ex.InnerException;
            }

            if (valueFromCache != null)
            {
                if (settings.FaultToleranceType == FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError && !valueFromCache.Persisted)
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

        private void CalculateExpirationsAndAddValueToVolatileCache(string key, object value, CacheSettings settings, bool persisted)
        {
            var softExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(settings.Duration);
            var hardExpiryDate = softExpiryDate.AddMinutes(2);
            InsertValueInVolatileCache(key, value, softExpiryDate, hardExpiryDate, persisted);
        }

        private void InsertValueInPersistentCache(string key, object value, CacheSettings settings)
        {
            if (settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache)
                _cache.InsertInPersistentCache(key, value);
        }

        private void InsertValueInVolatileCache(string key, object returnValue, DateTime softExpiryDate, DateTime hardExpiryDate, bool persisted)
        {
            _cache.Insert(
                    key,
                    new CacheValue
                    {
                        ExpiryDate = softExpiryDate,
                        Value = returnValue,
                        Persisted = persisted
                    },
                    hardExpiryDate);
        }

        private static bool IsValueFromVolatileCacheValid(CacheValue value)
        {
            return value.ExpiryDate >= TimeProvider.Current.UtcNow;
        }

        private void IncreaseSoftExpirationDateWhileQueryIsPerformed(string key, CacheValue value, bool persisted)
        {
            var softExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(10);
            InsertValueInVolatileCache(key, value.Value, softExpiryDate, softExpiryDate.AddMinutes(10), persisted);      
        }

        private CacheValue GetValueFromVolatileCache(string key)
        {
            return _cache.Get(key) as CacheValue;
        }

        private string GetInvocationKey(IInvocation invocation, CacheSettings settings)
        {
            return  string.IsNullOrEmpty(settings.Key) ? _helper.GetCallIdentifier(invocation) : settings.Key;
        }

        private static object GetActualResult(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                throw new CacheMethodInvocationException(ex);
            }
            return invocation.ReturnValue;
        }
    }
}
