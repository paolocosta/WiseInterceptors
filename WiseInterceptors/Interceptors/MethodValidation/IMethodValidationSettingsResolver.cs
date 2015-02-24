using System.Reflection;

namespace WiseInterceptors.Interceptors.MethodValidation
{
    public interface IMethodValidationSettingsResolver
    {
        bool BlockDefaultValueParameters(MethodInfo methodInfo, object[] arguments);
        bool BlockDefaultResult(MethodInfo methodInfo, object[] arguments);
    }
}
