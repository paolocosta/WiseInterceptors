using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.CircuitBreaker
{
    public class CircuitBreakerSettingsAttribute:Attribute
    {
        public CircuitBreakerSettingsAttribute()
        {
            ExceptionsBeforeBreak = 3;
            RetryingPeriodInSeconds = 60;
            BreakingPeriodInSeconds = 10;
            ExceptionType = typeof(TimeoutException);
            VaryByMethodArgs = 0;// string.Empty;
        }
        
        /// <summary>
        /// The number of consecutive exceptions that have to be raised before triggering
        /// the circuit breaker
        /// </summary>
        public int ExceptionsBeforeBreak { get; set; }

        /// <summary>
        /// The time span in which the consecutive exception will have to be raised 
        /// before triggering the circuit breaker. If set to one minute, it means that 
        /// if ExceptionsBeforeBreak consecutive exceptions will be triggered in one minute the
        /// circuit breaker will be activated
        /// </summary>
        public int RetryingPeriodInSeconds { get; set; }

        /// <summary>
        /// The time span for which the circuit breaker will return an exception
        /// without calling the method
        /// </summary>
        public int BreakingPeriodInSeconds { get; set; }

        /// <summary>
        /// Type of exception (or derived from) that will trigger the circuit breaker
        /// </summary>
        public Type ExceptionType { get; set; }

        /// <summary>
        /// Comma separated list of method whose value will be used to 
        /// identify the circuit breaker
        /// </summary>
        public int VaryByMethodArgs { get; set; }
    }
}
