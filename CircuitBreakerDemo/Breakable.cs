using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.CircuitBreaker;
using System.Reflection;

namespace CircuitBreakerDemo
{
    public class Breakable 
    {
        [CircuitBreakable]
        public virtual string HopeGetSomething()
        {
            DateTime now = DateTime.Now;
            if (now.Second < 30)
            {
                System.Threading.Thread.Sleep(1000 * 4);
                throw new TimeoutException();
            }
            return "ok";
        }
    }

    public class CircuitBreakableAttribute:Attribute
    { }

    public class CircuitBreakerSettingsReader : ICircuitBreakerSettingsReader
    {
        public CircuitBreakerSettings GetSettings(System.Reflection.MethodInfo method, object[] arguments)
        {
            if (method.GetCustomAttributes<CircuitBreakableAttribute>().Any())
            {
                return new CircuitBreakerSettings
                {
                    RetryingPeriodInSeconds = 30,
                    ExceptionType = typeof(Exception),
                    ExceptionsBeforeBreak = 3,
                    BreakingPeriodInSeconds = 30
                };
            }
            else
                return null;
        }
    }
}
