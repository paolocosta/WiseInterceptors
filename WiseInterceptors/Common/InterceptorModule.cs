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
            builder.RegisterType<Helper>().As<IHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInvocationStrategySelector>().As<ICacheInvocationStrategySelector>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<CircuitBreakerInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<MethodValidationInterceptor>().InstancePerLifetimeScope();

            builder.RegisterType<AlwaysUsePersistentCacheInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.AlwaysUsePersistentCache).InstancePerLifetimeScope();
            builder.RegisterType<ConsiderSoftlyExpiredValuesInCaseOfErrorsInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.ConsiderSoftlyExpiredValuesInCaseOfErrors).InstancePerLifetimeScope();
            builder.RegisterType<FailFastCacheInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.FailFastWithNoRecovery).InstancePerLifetimeScope();
            builder.RegisterType<UsePersistentCacheOnlyInCaseOfErrorInvocationManager>().Keyed<CacheInvocationManager>(FaultToleranceEnum.UsePersistentCacheOnlyInCaseOfError).InstancePerLifetimeScope();
            builder.Register<Func<FaultToleranceEnum, CacheInvocationManager>>(c => {
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return request => (CacheInvocationManager)ctx.ResolveKeyed<CacheInvocationManager>(request);
                }
            });
        }
    }
}