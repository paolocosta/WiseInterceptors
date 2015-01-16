﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        ICache _Cache;

        public CacheInterceptor(ICache cache)
        {
            _Cache = cache;
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
                var key = GetCacheKey(invocation);
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

        private string GetCacheKey(IInvocation invocation)
        {
            return string.Format("{0}_{1}", invocation.Method.DeclaringType.FullName, SerializeArguments(invocation));
        }

        private static string SerializeArguments(IInvocation invocation)
        {
            if(invocation.Arguments.Count()==0)
            {
                return "";
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(invocation.Arguments);
        }
    }
}