using System.Reflection;

namespace WiseInterceptors.Interceptors.Logger
{
    public interface ILogger
    {
        void Log(LogInformation logInformation);

        LogSettings GetLogSettings(MethodInfo methodInfo, object[] p);
    }
}
