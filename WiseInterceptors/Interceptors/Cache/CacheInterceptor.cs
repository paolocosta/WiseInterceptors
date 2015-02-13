using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using WiseInterceptors.Interceptors.Cache.Strategies;

namespace WiseInterceptors.Interceptors.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        readonly ICache _Cache;
        readonly IHelper _helper;
        readonly ICacheInvocationManagerFactory _invocationManagerFactory;

        public CacheInterceptor(
            ICache cache, 
            IHelper helper, 
            ICacheInvocationManagerFactory invocationManagerFactory)
        {
            _Cache = cache;
            _helper = helper;
            _invocationManagerFactory = invocationManagerFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = _invocationManagerFactory.Build(invocation)
                .GetInvocationResult(invocation);
        }
    }
}
