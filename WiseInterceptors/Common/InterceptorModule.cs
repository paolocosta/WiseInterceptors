using Autofac;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.Cache.CacheInvocationMethod;
using WiseInterceptors.Interceptors.CircuitBreaker;
using WiseInterceptors.Interceptors.MethodValidation;

namespace WiseInterceptors.Common
{
    public class InterceptorModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Helper>().As<IHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInvocationStrategySelector>().As<ICacheInvocationStrategySelector>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<CircuitBreakerInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<MethodValidationInterceptor>().InstancePerLifetimeScope();
        }
    }
}
