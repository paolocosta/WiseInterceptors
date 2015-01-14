using Castle.DynamicProxy;
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

            invocation.Proceed();
        }

        private string GetCacheKey(IInvocation invocation)
        {
            return string.Format("{0}_{1}_{2}", invocation.Method.DeclaringType.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(invocation.Arguments));
        }
    }
}
