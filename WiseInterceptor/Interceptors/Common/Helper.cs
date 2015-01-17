using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.Common
{
    public class Helper:IHelper
    {
        public string GetMethodIdentifier(IInvocation invocation)
        {
            return string.Format("{0}_{1}", invocation.Method.DeclaringType.FullName, invocation.Method.Name);
        }

        public string GetCallIdentifier(IInvocation invocation)
        {
            return string.Format("{0}_{1}_{2}", invocation.Method.DeclaringType.FullName, invocation.Method.Name, SerializeArguments(invocation));
        }

        private static string SerializeArguments(IInvocation invocation)
        {
            if (invocation.Arguments.Count() == 0)
            {
                return "";
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(invocation.Arguments);
        }
    }
}
