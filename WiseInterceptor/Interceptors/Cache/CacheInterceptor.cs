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
        public IHelper Helper { private get; set; }

        public CacheInterceptor(ICache cache)
        {
            _Cache = cache;
            Helper = new Helper();  //Default implementation if not specified by means of the SetHelperMethod
        }

        public void Intercept(IInvocation invocation)
        {
            CacheSettingsAttribute settings = Helper.GetInvocationMethodAttribute<CacheSettingsAttribute>(invocation);
            
            if (settings == null)
            {
                //No attribute no cache
                invocation.Proceed();
            }
            else
            {
                CheckNotVoidReturnType(invocation);

                var key = Helper.GetCallIdentifier(invocation);
                var value = _Cache.Get(key) as CacheValue;

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
                    }
                }

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

        private void CheckNotVoidReturnType(IInvocation invocation)
        {
            if (Helper.IsReturnTypeVoid(invocation))
            {
                throw new InvalidOperationException(string.Format("Cache was added to method {0}.{1} but it is not allowed as it returns void",
                    invocation.Method.DeclaringType.FullName,
                    invocation.Method.Name
                    ));
            }
        }
    }
}
