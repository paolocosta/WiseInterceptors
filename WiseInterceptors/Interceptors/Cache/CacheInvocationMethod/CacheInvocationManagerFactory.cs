using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache.Strategies;

namespace WiseInterceptors.Interceptors.Cache.CacheInvocationMethod
{
    public class CacheInvocationManagerFactory : ICacheInvocationManagerFactory
    {
        readonly ICache _cache;
        readonly IHelper _helper;

        public CacheInvocationManagerFactory(ICache cache, IHelper helper)
        {
            _cache = cache;
            _helper = helper;
        }

        public ICacheInvocationManager Build(IInvocation invocation)
        {
            this.FailIfInterceptedMethodReturnsVoid(invocation);
            CacheSettings settings = _cache.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);

            switch (settings.FaultToleranceType)
            {
                case FaultToleranceEnum.FailFastWithNoRecovery: 
                    return new FailFastCacheInvocationManager(_cache, _helper, settings); 
                    break;
                case FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors: 
                    return new ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager(_cache, _helper, settings); 
                    break;
                case FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError: 
                    return new UsePersistentCacheOnlyInCaseOfErrorInvocationManager(_cache, _helper, settings); 
                    break;
                case FaultToleranceEnum.AlwaysUsePersistentCache: 
                    return new AlwaysUsePersistentCacheInvocationManager(_cache, _helper, settings); 
                    break;
            }

            return null;
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
