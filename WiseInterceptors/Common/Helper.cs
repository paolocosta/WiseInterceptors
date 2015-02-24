using System;
using System.Linq;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace WiseInterceptors.Common
{
    public class Helper:IHelper
    {
        public string GetMethodIdentifier(IInvocation invocation)
        {
            var method = invocation.GetConcreteMethodInvocationTarget();
            return string.Format("{0}_{1}", method.DeclaringType.FullName, method.Name);
        }

        public string GetUnivoqueCallIdentifier(IInvocation invocation)
        {
            return string.Format("{0}_{1}", GetMethodIdentifier(invocation), SerializeArguments(invocation));
        }

        public T GetInvocationMethodAttribute<T>(IInvocation invocation) where T : Attribute
        {
            T value =
                invocation.GetConcreteMethodInvocationTarget().GetCustomAttributes(typeof(T), false) 
                .FirstOrDefault() as T;
            return value;
        }

        private static string SerializeArguments(IInvocation invocation)
        {
            if (invocation.Arguments.Count() == 0)
            {
                return "";
            }
            return JsonConvert.SerializeObject(invocation.Arguments);
        }

        public bool IsInvocationMethodReturnTypeVoid(IInvocation invocation)
        {
            return invocation.Method.ReturnType == typeof(void);
        }

        public object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public bool HasInvocationMethodAttribute<T>(IInvocation invocation) where T : Attribute
        {
            return invocation.GetConcreteMethodInvocationTarget().GetCustomAttributes(typeof(T), false).Any();
        }

        public string GetMethodDescription(IInvocation invocation)
        {
            return string.Format("{0}.{1}",
                        invocation.Method.DeclaringType.FullName,
                        invocation.Method.Name);
        }
    }
}
