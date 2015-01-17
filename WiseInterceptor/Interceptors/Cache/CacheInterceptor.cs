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
            _Helper = new Helper();
        }

        /// <summary>
        /// Not exposed poor man's DI for testing purpose
        /// </summary>
        /// <param name="helper"></param>
        internal void SetHelper(IHelper helper)
        {
            _Helper = helper;
        }

        public void Intercept(IInvocation invocation)
        {
            CacheSettingsAttribute settings =
                invocation.MethodInvocationTarget.GetCustomAttributes(typeof(CacheSettingsAttribute), false)
                .FirstOrDefault() as CacheSettingsAttribute;

            if (settings == null)
            {
                invocation.Proceed();
            }
            else
            {
                var key = _Helper.GetCallIdentifier(invocation);
                var value = _Cache.Get(key) as CacheValue;
                bool proceed = true;
                if (value != null)
                {
                    if (value.ExpiryDate > _Cache.Now())
                    {
                        invocation.ReturnValue = value.Value;
                        return;
                    }
                    else
                    {
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
                if (proceed)
                {
                    invocation.Proceed();
                    _Cache.Insert(
                            key,
                            new CacheValue
                            {
                                ExpiryDate = _Cache.Now().AddSeconds(settings.SoftDuration),
                                Value = invocation.ReturnValue
                            },
                            _Cache.Now().AddSeconds(settings.Duration));
                }
            }
        }
    }
}
