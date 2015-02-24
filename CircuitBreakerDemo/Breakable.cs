using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using WiseInterceptors.Interceptors.CircuitBreaker;

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
                Thread.Sleep(1000 * 4);
                throw new TimeoutException();
            }
            return "ok";
        }
    }

    public class CircuitBreakableAttribute:Attribute
    { }

    public class CircuitBreakerSettingsReader : ICircuitBreakerSettingsReader
    {
        public CircuitBreakerSettings GetSettings(MethodInfo method, object[] arguments)
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
