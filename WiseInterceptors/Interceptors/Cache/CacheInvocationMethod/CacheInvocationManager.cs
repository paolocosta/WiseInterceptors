using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.Cache.CacheInvocationMethod
{
    public abstract class CacheInvocationManager
    {
        protected readonly ICache _cache;
        protected readonly IHelper _helper;
        
        public CacheInvocationManager(ICache cache, IHelper helper)
        {
            _cache = cache;
            _helper = helper;
        }

        protected abstract bool IsPersistedByDefault { get; }

        public virtual object GetInvocationResult(IInvocation invocation)
        {
            this.FailIfInterceptedMethodReturnsVoid(invocation);

            var _settings = _cache.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);
            if (!_settings.UseCache)
            {
                return GetActualResult(invocation);
            }
            else
            {
                var key = GetInvocationKey(invocation, _settings);

                var valueFromCache = RetrieveValueFromVolatileCache(key);

                if (valueFromCache != null)
                {
                    if (IsValueFromVolatileCacheValid(valueFromCache))
                    {
                        return valueFromCache.Value;
                    }
                    else
                    {
                        IncreaseSoftExpirationDateWhileActualQueryIsPerformedSoOnlyThisThreadRunsTheActualQuery(key, valueFromCache, false);
                    }
                }

                lock (string.Intern(key))
                {
                    try
                    {
                        return PerformActualCallProcess(invocation, _settings, key);
                    }
                    catch (CacheMethodInvocationException ex)
                    {
                        return HandleInvocationException(_settings, key, valueFromCache, ex);
                    }
                }
            }
        }

        protected abstract object HandleInvocationException(CacheSettings settings, string key, CacheValue valueFromCache, CacheMethodInvocationException ex);

        protected abstract void InsertValueInAnyRequiredCache(string key, object value, CacheSettings settings, bool persisted, DateTime softExpiryDate, DateTime hardExpiryDate);

        private object PerformActualCallProcess(IInvocation invocation, CacheSettings settings, string key)
        {
            var result = GetActualResult(invocation);
            CalculateExpirationsAndAddValueToVolatileCache(key, result, settings, IsPersistedByDefault);
            
            return result;
        }
        
        protected void CalculateExpirationsAndAddValueToVolatileCache(string key, object value, CacheSettings settings, bool persisted)
        {
            var softExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(settings.Duration);
            var hardExpiryDate = softExpiryDate.AddMinutes(2);
            InsertValueInAnyRequiredCache(key, value, settings, persisted, softExpiryDate, hardExpiryDate);
        }

        protected void InsertValueInPersistentCache(string key, object value, CacheSettings settings)
        {
             _cache.InsertInPersistentCache(key, value);
        }

        protected void InsertValueInVolatileCache(string key, object returnValue, DateTime softExpiryDate, DateTime hardExpiryDate, bool persisted)
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

        private void IncreaseSoftExpirationDateWhileActualQueryIsPerformedSoOnlyThisThreadRunsTheActualQuery(string key, CacheValue value, bool persisted)
        {
            var softExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(10);
            InsertValueInVolatileCache(key, value.Value, softExpiryDate, softExpiryDate.AddMinutes(10), persisted);
        }

        private CacheValue RetrieveValueFromVolatileCache(string key)
        {
            return _cache.Get(key) as CacheValue;
        }

        private string GetInvocationKey(IInvocation invocation, CacheSettings settings)
        {
            return string.IsNullOrEmpty(settings.Key) ? _helper.GetUnivoqueCallIdentifier(invocation) : settings.Key;
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

        private void FailIfInterceptedMethodReturnsVoid(IInvocation invocation)
        {
            if (_helper.IsInvocationMethodReturnTypeVoid(invocation))
            {
                throw new InvalidOperationException(string.Format("Cache interceptor was added to method {0} but it is not allowed as it returns void",
                    _helper.GetMethodDescription(invocation)
                    ));
            }
        }
    }
}
