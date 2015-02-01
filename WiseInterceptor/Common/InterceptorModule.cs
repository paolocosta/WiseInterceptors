using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Cache;
using WiseInterceptor.Interceptors.CircuitBreaker;
using WiseInterceptor.Interceptors.MethodValidation;

namespace WiseInterceptor.Common
{
    public class InterceptorModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Helper>().As<IHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CacheInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<CircuitBreakerInterceptor>().InstancePerLifetimeScope();
            builder.RegisterType<MethodValidationInterceptor>().InstancePerLifetimeScope();
        }
    }
}
