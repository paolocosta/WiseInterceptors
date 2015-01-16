using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Cache;
using Autofac.Extras.DynamicProxy2;

namespace CacheDemo
{
    class Program
    {
        static void Main(string[] args)
        {
             var container = BuildContainer();

             using (var scope = container.BeginLifetimeScope())
             {
                 ICachable cachable = scope.Resolve<ICachable>();
                 var now = cachable.Now();
                 System.Threading.Thread.Sleep(1000);
                 var now2 = cachable.Now();

                 Console.WriteLine(now.ToString() + " " + now2.ToString());
                 Console.ReadLine();
             }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new CacheInterceptor(new Cache()));

            builder.RegisterType<Cachable>().As<ICachable>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(CacheInterceptor));

            var container = builder.Build();
            return container;
        }
    }
}
