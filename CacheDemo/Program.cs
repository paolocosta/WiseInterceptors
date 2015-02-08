using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiseInterceptors.Common;
using Autofac.Extras.DynamicProxy2;
using WiseInterceptors.Interceptors.Cache;

namespace CacheDemo
{
    static class Program
    {
        public static Form1 _Form1;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var container = BuildContainer();
            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                _Form1 = scope.Resolve<Form1>();
                Application.Run(_Form1);
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<InterceptorModule>();
            builder.RegisterType<Cache>().As<ICache>();
            builder.RegisterType<Form1>();

            builder.RegisterType<TimeGetter>()
            .EnableClassInterceptors()
            .InterceptedBy(typeof(CacheInterceptor));

            var container = builder.Build();
            return container;
        }
    }
}
