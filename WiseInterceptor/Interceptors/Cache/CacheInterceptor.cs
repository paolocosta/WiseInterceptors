using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Common;

namespace WiseInterceptor.Interceptors.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        ICache _Cache;
        IHelper _Helper;

        public CacheInterceptor(ICache cache)
        {
            _Cache = cache;
            _Helper = new Helper();  //There's no need to use DI for this class
        }

        public void Intercept(IInvocation invocation)
        {
            CacheSettingsAttribute settings =
                invocation.MethodInvocationTarget.GetCustomAttributes(typeof(CacheSettingsAttribute), false)
                .FirstOrDefault() as CacheSettingsAttribute;

            if (settings == null)
            {
                //No attribute no cache
                invocation.Proceed();
            }
            else
            {
                CheckNotVoidReturnType(invocation);

                var key = _Helper.GetCallIdentifier(invocation);
                var value = _Cache.Get(key) as CacheValue;
                bool proceed = true;
                if (value != null)
                {
                    //Check if the soft expiry date is valid
                    if (value.ExpiryDate > _Cache.Now())
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
                                ExpiryDate = _Cache.Now().AddYears(1),
                                Value = value.Value
                            },
                            _Cache.Now().AddYears(1));

                        return;
                    }
                }
                //proceed is true either when the data was not found in cache or were "softly" expired.
                if (proceed)
                {
                    lock (string.Intern(key))
                    {
                        invocation.Proceed();
                        _Cache.Insert(
                                key,
                                new CacheValue
                                {
                                    ExpiryDate = _Cache.Now().AddSeconds(settings.Duration),
                                    Value = invocation.ReturnValue
                                },
                                _Cache.Now().AddSeconds(settings.Duration).AddMinutes(2));
                    }
                }
            }
        }

        private static void CheckNotVoidReturnType(IInvocation invocation)
        {
            if (invocation.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException(string.Format("Cache was added to method {0}.{1} but it is not allowed as it returns void",
                    invocation.Method.DeclaringType.FullName,
                    invocation.Method.Name
                    ));
            }
        }
    }
}
