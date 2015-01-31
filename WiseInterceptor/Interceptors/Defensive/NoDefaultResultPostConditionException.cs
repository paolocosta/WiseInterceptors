﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Interceptors.Common;

namespace WiseInterceptor.Interceptors.Defensive
{
    public class NoDefaultResultPostConditionException:Exception
    {
        public NoDefaultResultPostConditionException(IInvocation invocation, IHelper helper)
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
