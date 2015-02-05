using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Common
{
    public interface IHelper
    {
        string GetMethodIdentifier(IInvocation invocation);
        string GetMethodDescription(IInvocation invocation);
        string GetCallIdentifier(IInvocation invocation);
        T GetInvocationMethodAttribute<T>(IInvocation invocation) where T : Attribute;
        bool HasInvocationAttribute<T>(IInvocation invocation) where T : Attribute;
        bool IsReturnTypeVoid(IInvocation invocation);
        object GetDefaultValue(Type type);
    }
}
