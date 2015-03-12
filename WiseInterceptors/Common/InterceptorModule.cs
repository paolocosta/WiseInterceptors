using Autofac;
using System;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using WiseInterceptors.Interceptors.Cache.Strategies;
using WiseInterceptors.Interceptors.CircuitBreaker;
using WiseInterceptors.Interceptors.MethodValidation;

namespace WiseInterceptors.Common
{
    public class InterceptorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Helper>().As<IHelper>().SingleInstance();
            builder.RegisterType<CacheInvocationStrategySelector>().As<ICacheInvocationStrategySelector>().SingleInstance();
            builder.RegisterType<CacheInterceptor>().SingleInstance();
            builder.RegisterType<CircuitBreakerInterceptor>().SingleInstance();
            builder.RegisterType<MethodValidationInterceptor>().SingleInstance();

            builder.RegisterType<AlwaysUsePersistentCacheInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.AlwaysUsePersistentCache).SingleInstance();
            builder.RegisterType<ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors).SingleInstance();
            builder.RegisterType<FailFastCacheInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.FailFastWithNoRecovery).SingleInstance();
            builder.RegisterType<UsePersistentCacheOnlyInCaseOfErrorInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError).SingleInstance();
            builder.Register<Func<FaultToleranceEnum, CacheInvocationManager>>(c => {
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return request => (CacheInvocationManager)ctx.ResolveKeyed<CacheInvocationManager>(request);
                }
            });
        }
    }
}