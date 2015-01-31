using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Common;

namespace WiseInterceptor.Interceptors.MethodValidation
{
    public class DefaultParameterValuePreConditionException:ApplicationException
    {
        public DefaultParameterValuePreConditionException(IInvocation invocation, IHelper helper)
            : base(GetExceptionDescription(invocation, helper))
        {
        }

        private static string GetExceptionDescription(IInvocation invocation, IHelper helper)
        {
            StringBuilder message = new StringBuilder();
            for (int i = 0; i < invocation.Arguments.Count(); i++)
            {
                if (invocation.Arguments[i].Equals(helper.GetDefaultValue(invocation.Arguments[i].GetType())))
                {
                    message.Append(string.Format("Method {0} of type {1} is called with default value on parameter {2}. ",
                        invocation.MethodInvocationTarget.Name,
                        invocation.MethodInvocationTarget.DeclaringType.Name,
                        invocation.MethodInvocationTarget.GetParameters()[i].Name));
                }
            }
            return message.ToString();
        }
    }
}
