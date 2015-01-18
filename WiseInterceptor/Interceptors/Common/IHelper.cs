using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.Common
{
    public interface IHelper
    {
        string GetMethodIdentifier(IInvocation invocation);
        string GetCallIdentifier(IInvocation invocation);
        T GetInvocationMethodAttribute<T>(IInvocation invocation) where T : class;
        bool IsReturnTypeVoid(IInvocation invocation);
    }
}
