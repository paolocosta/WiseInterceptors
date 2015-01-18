using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.CircuitBreaker;

namespace CircuitBreakerDemo
{
    public class Breakable 
    {
        [CircuitBreakerSettings(
            ExceptionType = typeof(Exception), 
            RetryingPeriodInSeconds=60, 
            BreakingPeriodInSeconds=10, 
            ExceptionsBeforeBreak=5)]
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
