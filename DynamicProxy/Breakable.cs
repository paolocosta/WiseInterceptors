using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.CircuitBreaker;

namespace DynamicProxy
{
    public class Breakable : DynamicProxy.IBreakable
    {

        [CircuitBreakerSettings(ExceptionType = typeof(Exception))]
        public virtual string HopeGetSomething()
        {
            DateTime now = DateTime.Now;
            if (now.Second < 30)
            {
                System.Threading.Thread.Sleep(1000 * 3);
                throw new TimeoutException();
            }
            return "ok";
        }
    }
}
