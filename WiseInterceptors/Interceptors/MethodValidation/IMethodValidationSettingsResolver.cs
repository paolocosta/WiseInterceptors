using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Interceptors.MethodValidation
{
    public interface IMethodValidationSettingsResolver
    {
        bool BlockDefaultValueParameters(MethodInfo methodInfo, object[] arguments);
        bool BlockDefaultResult(MethodInfo methodInfo, object[] arguments);
    }
}
