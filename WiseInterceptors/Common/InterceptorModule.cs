using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Interceptors.Cache;
using WiseInterceptors.Interceptors.CircuitBreaker;
using WiseInterceptors.Interceptors.MethodValidation;

namespace WiseInterceptors.Common
{
    public class InterceptorModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Helper>().As<IHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInvocationManager>().As<ICacheInvocationManager>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<CircuitBreakerInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<MethodValidationInterceptor>().InstancePerLifetimeScope();

            
        }
    }
}
