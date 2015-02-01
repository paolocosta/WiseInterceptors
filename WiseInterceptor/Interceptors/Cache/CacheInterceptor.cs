using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Common;

namespace WiseInterceptor.Interceptors.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        ICache _Cache;
        IHelper _helper;
        ICacheSettingsReader _cacheSettingsReader;

        public CacheInterceptor(ICache cache, IHelper helper, ICacheSettingsReader cacheSettingsReader)
        {
            _Cache = cache;
            _helper = helper;
            _cacheSettingsReader = cacheSettingsReader;
        }

        public void Intercept(IInvocation invocation)
        {
            CacheSettings settings = _cacheSettingsReader.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);
            
            if (settings == null)
            {
                //No attribute no cache
                invocation.Proceed();
            }
            else
            {
                CheckNotVoidReturnType(invocation);

                var key = _helper.GetCallIdentifier(invocation);
                var value = _Cache.Get(key) as CacheValue;

                if (value != null)
                {
                    //Check if the soft expiry date is valid
                    if (value.ExpiryDate > TimeProvider.Current.UtcNow)
                    {
                        invocation.ReturnValue = value.Value;
                        return;
                    }
                    else
                    {
                        //increase the soft expiry date and insert the value in cache. This means that this request 
                        //will be in charge of refreshing the query while the others can still see query the cache 
                        _Cache.Insert(
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
                     
                    invocation.Proceed();
                    _Cache.Insert(
                            key,
                            new CacheValue
                            {
                                ExpiryDate = TimeProvider.Current.UtcNow.AddSeconds(settings.Duration),
                                Value = invocation.ReturnValue
                            },
                            TimeProvider.Current.UtcNow.AddSeconds(settings.Duration).AddMinutes(2));
                }
            }
        }

        private void CheckNotVoidReturnType(IInvocation invocation)
        {
            if (_helper.IsReturnTypeVoid(invocation))
            {
                throw new InvalidOperationException(string.Format("Cache was added to method {0}.{1} but it is not allowed as it returns void",
                    invocation.Method.DeclaringType.FullName,
                    invocation.Method.Name
                    ));
            }
        }
    }
}
