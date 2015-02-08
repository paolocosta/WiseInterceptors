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
        object GetResult(IInvocation invocation); 
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

        private void CheckReturnTypeIsNotVoid(IInvocation invocation)
        {
            if (_helper.IsReturnTypeVoid(invocation))
            {
                throw new InvalidOperationException(string.Format("Cache interceptor was added to method {0} but it is not allowed as it returns void",
                    _helper.GetMethodDescription(invocation)
                    ));
            }
        }

        public object GetResult(IInvocation invocation)
        {
            this.CheckReturnTypeIsNotVoid(invocation);

            CacheSettings settings = _cache.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);

            if (!settings.UseCache)
            {
                return GetRealResult(invocation);
            }
            else
            {
                var key = GetInvocationKey(invocation, settings);
                
                var valueFromCache = GetCacheValue(key);

                if (valueFromCache != null)
                {
                    if (IsCacheValueValid(valueFromCache))
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
                        var result = GetRealResult(invocation);
                        AddValueToVolatileCache(key, result, settings, settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache);
                        AddValueToPersistentCache(key, result, settings);
                        return result;
                    }
                    catch (CacheMethodInvocationException ex)
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
                                AddValueToVolatileCache(key, valueFromCache.Value, settings, true);
                            }
                            return valueFromCache.Value;
                        }

                        object valueFromPersistentCache = _cache.GetFromPersistentCache(key);
                        
                        if (valueFromPersistentCache != null)
                        {
                            AddValueToVolatileCache(key, valueFromPersistentCache, settings, true);
                            return valueFromPersistentCache;
                        }

                        throw ex.InnerException;
                    }
                }
            }
        }

        private void AddValueToVolatileCache(string key, object value, CacheSettings settings, bool persisted)
        {
            var softExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(settings.Duration);
            var hardExpiryDate = softExpiryDate.AddMinutes(2);
            InsertValueInCache(key, value, softExpiryDate, hardExpiryDate, persisted);
        }

        private void AddValueToPersistentCache(string key, object value, CacheSettings settings)
        {
            if (settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache)
                _cache.InsertInPersistentCache(key, value);
        }

        private void InsertValueInCache(string key, object returnValue, DateTime softExpiryDate, DateTime hardExpiryDate, bool persisted)
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

        private static bool IsCacheValueValid(CacheValue value)
        {
            return value.ExpiryDate >= TimeProvider.Current.UtcNow;
        }

        private void IncreaseSoftExpirationDateWhileQueryIsPerformed(string key, CacheValue value, bool persisted)
        {
            var softExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(10);
            InsertValueInCache(key, value.Value, softExpiryDate, softExpiryDate.AddMinutes(10), persisted);      
        }

        private CacheValue GetCacheValue(string key)
        {
            return _cache.Get(key) as CacheValue;
        }

        private string GetInvocationKey(IInvocation invocation, CacheSettings settings)
        {
            return  string.IsNullOrEmpty(settings.Key) ? _helper.GetCallIdentifier(invocation) : settings.Key;
        }

        private static object GetRealResult(IInvocation invocation)
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
