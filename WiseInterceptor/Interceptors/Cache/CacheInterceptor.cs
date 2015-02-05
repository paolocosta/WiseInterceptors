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
        readonly ICache _Cache;
        readonly IHelper _helper;
        readonly ICacheSettingsReader _cacheSettingsReader;
        readonly ICacheInvocationManager _invocationManager;

        public CacheInterceptor(
            ICache cache, 
            IHelper helper, 
            ICacheSettingsReader cacheSettingsReader,
            ICacheInvocationManager invocationManager)
        {
            _Cache = cache;
            _helper = helper;
            _cacheSettingsReader = cacheSettingsReader;
            _invocationManager = invocationManager;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = _invocationManager.GetResult(invocation);
        }
    }
}
