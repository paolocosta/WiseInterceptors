using System;

namespace WiseInterceptors.Interceptors.CircuitBreaker
{
    public class CircuitBreakerSettings
    {
        public CircuitBreakerSettings()
        {
            ExceptionsBeforeBreak = 3;
            RetryingPeriodInSeconds = 60;
            BreakingPeriodInSeconds = 10;
            ExceptionType = typeof(TimeoutException);
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
    }
}
