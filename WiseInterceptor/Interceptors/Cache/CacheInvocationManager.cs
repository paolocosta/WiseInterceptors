using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Common;

namespace WiseInterceptor.Interceptors.Cache
{
    public interface ICacheInvocationManager
    {
        object GetResult(IInvocation invocation); 
    }

    public class CacheInvocationManager : ICacheInvocationManager
    {
        readonly ICache _cache;
        readonly IHelper _helper;
        readonly ICacheSettingsReader _cacheSettingsReader;

        public CacheInvocationManager(ICache cache, IHelper helper, ICacheSettingsReader cacheSettingsReader)
        {
            _cache = cache;
            _helper = helper;
            _cacheSettingsReader = cacheSettingsReader;
        }

            private void CheckNotVoidReturnType(IInvocation invocation)
            {
                if (_helper.IsReturnTypeVoid(invocation))
                {
                    throw new InvalidOperationException(string.Format("Cache was added to method {0} but it is not allowed as it returns void",
                        _helper.GetMethodDescription(invocation)
                        ));
                }
            }
        

        public object GetResult(IInvocation invocation)
        {
            CacheSettings settings = _cacheSettingsReader.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);

            if (!settings.UseCache)
            {
                invocation.Proceed();
                return invocation.ReturnValue;
            }
            else
            {
                this.CheckNotVoidReturnType(invocation);
                var key = _helper.GetCallIdentifier(invocation);
                var value = _cache.Get(key) as CacheValue;

                if (value != null)
                {
                    //Check if the soft expiry date is valid
                    if (value.ExpiryDate > TimeProvider.Current.UtcNow)
                    {
                        return value.Value;
                    }
                    else
                    {
                        //increase the soft expiry date and insert the value in cache. This means that this request 
                        //will be in charge of refreshing the query while the others can still see query the cache 
                        _cache.Insert(
                            key,
                            new CacheValue
                            {
                                ExpiryDate = TimeProvider.Current.UtcNow.AddYears(1),
                                Value = value.Value
                            },
                            TimeProvider.Current.UtcNow.AddYears(1));
                    }
                }

                lock (string.Intern(key))
                {
                    try
                    {
                        invocation.Proceed();
                        _cache.Insert(
                                key,
                                new CacheValue
                                {
                                    ExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(settings.Duration),
                                    Value = invocation.ReturnValue
                                },
                                TimeProvider.Current.UtcNow.AddSeconds(settings.Duration).AddMinutes(2));

                        if (settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache)
                            _cache.InsertInPersistantCache(key, invocation.ReturnValue);

                        return invocation.ReturnValue;
                    }
                    catch
                    {
                        if (value != null)
                        {
                            if (settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache || settings.FaultToleranceType == FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError)
                            {
                                _cache.InsertInPersistantCache(key, value.Value);
                            }
                            return value.Value;
                        }
                        if (settings.FaultToleranceType == FaultToleranceEnum.AlwaysUsePersistentCache || settings.FaultToleranceType == FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError)
                        {
                            object valueFromPersistantCache = _cache.GetFromPersistantCache(key);
                            if (valueFromPersistantCache != null)
                            {
                                return valueFromPersistantCache;
                            }
                        }
                        throw;
                    }
                }
            }
        }

        
    }
}
