using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using WiseInterceptor.Utilities;
using WiseInterceptor.Interceptors.Common;

namespace WiseInterceptor.Interceptors.Defensive
{
    public class DefensiveInterceptor:IInterceptor
    {
        IHelper _helper;
        
        public DefensiveInterceptor()
        {
            _helper = new Helper();
        }
        
        public void Intercept(IInvocation invocation)
        {
            CheckPreconditions(invocation);
            invocation.Proceed();   
        }

        private void CheckPreconditions(IInvocation invocation)
        {
            if (invocation.Arguments.Count() > 0 && invocation.MethodInvocationTarget.GetCustomAttributes<BlockDefaultValuesAttribute>().Count() > 0)
            {
                if (invocation.Arguments.Where(p => p.Equals(_helper.GetDefaultValue(p.GetType()))).Any())
                {
                    throw new BlockDefaultValuePreconditionViolatedException(invocation, _helper);
                }
            }
        }
    }
}
