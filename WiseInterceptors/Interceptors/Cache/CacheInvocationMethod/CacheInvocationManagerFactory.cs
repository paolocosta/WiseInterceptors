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
        readonly FaultToleranceEnum _faultToleranceStrategy;


        public CacheInvocationManagerFactory(ICache cache, IHelper helper)
        {
            _cache = cache;
            _helper = helper;
            _faultToleranceStrategy = _cache.GetFaultToleranceStrategy();
        }

        public ICacheInvocationManager Build(IInvocation invocation)
        {
            this.FailIfInterceptedMethodReturnsVoid(invocation);
            
            switch (_faultToleranceStrategy)
            {
                case FaultToleranceEnum.FailFastWithNoRecovery: 
                    return new FailFastCacheInvocationManager(_cache, _helper); 
                    break;
                case FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors: 
                    return new ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager(_cache, _helper); 
                    break;
                case FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError: 
                    return new UsePersistentCacheOnlyInCaseOfErrorInvocationManager(_cache, _helper); 
                    break;
                case FaultToleranceEnum.AlwaysUsePersistentCache: 
                    return new AlwaysUsePersistentCacheInvocationManager(_cache, _helper); 
                    break;
            }

            throw new ApplicationException("    This line of code should never be reached. Maybe a new value has been added to the " + 
                                           "    FaultToleranceEnum but not handled by CacheInvocationManagerFactory                ");
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
