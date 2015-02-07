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
                var key = GetInvocationKey(invocation);
                var valueFromCache = GetCacheValue(key);

                if (valueFromCache != null)
                {
                    if (IsCacheValueValid(valueFromCache))
                    {
                        return valueFromCache.Value;
                    }
                    else
                    {
                        IncreaseSoftExpirationDate(key, valueFromCache);
                    }
                }

                lock (string.Intern(key))
                {
                    try
                    {
                        var result = GetRealResult(invocation);
                        AddResultToCache(invocation, settings, key);
                        return result;
                    }
                    catch (CacheMethodInvocationException ex)
                    {
                        if (IsAnyKindOfPersistentCacheUsed(settings))
                        {
                            if (valueFromCache != null)
                            {
                                _cache.InsertInPersistantCache(key, valueFromCache.Value);
                                return valueFromCache.Value;
                            }

                            object valueFromPersistantCache = _cache.GetFromPersistantCache(key);
                            if (valueFromPersistantCache != null)
                            {
                                return valueFromPersistantCache;
                            }
                        }
                        throw ex.InnerException;
                    }
                }
            }
        }

        private static bool IsAnyKindOfPersistentCacheUsed(CacheSettings settings)
        {
            return settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache || settings.FaultToleranceType == FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError;
        }

        private void AddResultToCache(IInvocation invocation, CacheSettings settings, string key)
        {
            _cache.Insert(
                    key,
                    new CacheValue
                    {
                        ExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(settings.Duration),
                        Value = GetRealResult(invocation)
                    },
                    TimeProvider.Current.UtcNow.AddSeconds(settings.Duration).AddMinutes(2));

            if (settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache)
                _cache.InsertInPersistantCache(key, invocation.ReturnValue);
        }

        private static bool IsCacheValueValid(CacheValue value)
        {
            return value.ExpiryDate >= TimeProvider.Current.UtcNow;
        }

        private void IncreaseSoftExpirationDate(string key, CacheValue value)
        {
            _cache.Insert(
                key,
                new CacheValue
                {
                    ExpiryDate = TimeProvider.Current.UtcNow.AddYears(1),
                    Value = value.Value
                },
                TimeProvider.Current.UtcNow.AddYears(1));
        }

        private CacheValue GetCacheValue(string key)
        {
            return _cache.Get(key) as CacheValue;
        }

        private string GetInvocationKey(IInvocation invocation)
        {
            return _helper.GetCallIdentifier(invocation);
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
