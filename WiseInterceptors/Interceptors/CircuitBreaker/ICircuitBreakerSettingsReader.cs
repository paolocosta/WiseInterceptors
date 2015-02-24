using System.Reflection;

namespace WiseInterceptors.Interceptors.CircuitBreaker
{
    public interface ICircuitBreakerSettingsReader
    {
        CircuitBreakerSettings GetSettings(MethodInfo method, object[] arguments);
    }
}
