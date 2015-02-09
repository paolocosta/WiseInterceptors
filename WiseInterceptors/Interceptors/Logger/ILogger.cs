using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Interceptors.Logger
{
    public interface ILogger
    {
        void Log(LogInformation logInformation);

        LogSettings GetLogSettings(System.Reflection.MethodInfo methodInfo, object[] p);
    }
}
