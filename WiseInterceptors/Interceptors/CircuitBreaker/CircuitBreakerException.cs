using System;

namespace WiseInterceptors.Interceptors.CircuitBreaker
{
    public class CircuitBreakerException:Exception
    {
        public CircuitBreakerException(Exception ex)
            : base(ex.Message, ex)
        { }
    }
}
