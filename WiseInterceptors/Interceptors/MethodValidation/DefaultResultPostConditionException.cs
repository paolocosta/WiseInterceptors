﻿using System;
using System.Text;
using Castle.DynamicProxy;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.MethodValidation
{
    public class DefaultResultPostConditionException:Exception
    {
        public DefaultResultPostConditionException(IInvocation invocation, IHelper helper)
            : base(GetExceptionDescription(invocation, helper))
        {
        }

        private static string GetExceptionDescription(IInvocation invocation, IHelper helper)
        {
            StringBuilder message = new StringBuilder();

            message.Append(string.Format("Method {0} of type {1} cannot return default value",
                invocation.MethodInvocationTarget.Name,
                invocation.MethodInvocationTarget.DeclaringType.Name));
            
            return message.ToString();
        }
    }
}
