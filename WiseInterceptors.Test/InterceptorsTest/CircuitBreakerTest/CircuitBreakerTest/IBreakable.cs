using System;
namespace WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest
{
    public interface IBreakable
    {
        void Call(bool raiseException);
    }
}
