using Autofac;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.CircuitBreaker;
using Autofac.Extras.DynamicProxy2;

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
                    IBreakable b = scope.Resolve<IBreakable>();
                    try
                    {
                        System.Threading.Thread.Sleep(100);
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

            builder.Register(c => new CircuitBreakerInterceptor(new Cache()));

            builder.RegisterType<Breakable>()
                .As<IBreakable>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(CircuitBreakerInterceptor));

            var container = builder.Build();
            return container;
        }
    }
}
