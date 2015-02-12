using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Common
{
    public interface IHelper
    {
        string GetMethodIdentifier(IInvocation invocation);
        string GetMethodDescription(IInvocation invocation);
        string GetUnivoqueCallIdentifier(IInvocation invocation);
        T GetInvocationMethodAttribute<T>(IInvocation invocation) where T : Attribute;
        bool HasInvocationMethodAttribute<T>(IInvocation invocation) where T : Attribute;
        bool IsInvocationMethodReturnTypeVoid(IInvocation invocation);
        object GetDefaultValue(Type type);
    }
}
