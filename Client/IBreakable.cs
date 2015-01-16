using System;
using WiseInterceptor.Interceptors.CircuitBreaker;

namespace CircuitBreakerDemo
{
    public interface IBreakable
    {
        string HopeGetSomething();
    }
}
