using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using WiseInterceptor.Common;

namespace WiseInterceptor.Interceptors.MethodValidation
{
    public class MethodValidationInterceptor:IInterceptor
    {
        IHelper _helper;

        public MethodValidationInterceptor()
        {
            _helper = new Helper();
        }
        
        public void Intercept(IInvocation invocation)
        {
            CheckPreconditions(invocation);
            invocation.Proceed();
            CheckPostConditions(invocation);    
        }

        private void CheckPreconditions(IInvocation invocation)
        {
            if (_helper.HasInvocationAttribute<BlockDefaultParameterValuesAttribute>(invocation))
            {
                if (invocation.Arguments.Where(p => p.Equals(_helper.GetDefaultValue(p.GetType()))).Any())
                {
                    throw new DefaultParameterValuePreConditionException(invocation, _helper);
                }
            }
        }

        private void CheckPostConditions(IInvocation invocation)
        {
            if (_helper.HasInvocationAttribute<BlockDefaultResultAttribute>(invocation))
            {
                if (invocation.ReturnValue.Equals(_helper.GetDefaultValue(invocation.ReturnValue.GetType())))
                {
                    throw new DefaultResultPostConditionException(invocation, _helper);
                }
            }
        }
    }
}
