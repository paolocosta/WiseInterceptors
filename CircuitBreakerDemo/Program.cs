using System;
using System.Threading;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.CircuitBreaker;

namespace CircuitBreakerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = BuildContainer();
          
            using (var scope = container.BeginLifetimeScope())
            {

                for (int i = 0; i < 10000; i++)
                {
                    var b = scope.Resolve<Breakable>();
                    try
                    {
                        Thread.Sleep(1000);
                        b.HopeGetSomething();
                        Console.WriteLine(string.Format("{0} OK", DateTime.Now.TimeOfDay.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("{0} {1}", DateTime.Now.TimeOfDay.ToString(), ex.GetType().ToString()));
                    }
                }
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<InterceptorModule>();
            builder.RegisterType<Cache>().As<ICache>();
            builder.RegisterType<CircuitBreakerSettingsReader>().As<ICircuitBreakerSettingsReader>();
            

            builder.RegisterType<Breakable>()
            .EnableClassInterceptors()
            .InterceptedBy(typeof(CircuitBreakerInterceptor));

            var container = builder.Build();
            return container;
        }
    }
}
