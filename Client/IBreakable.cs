using System;
using WiseInterceptor.Interceptors.CircuitBreaker;
namespace DynamicProxy
{
    public interface IBreakable
    {
        string HopeGetSomething();
    }
}
