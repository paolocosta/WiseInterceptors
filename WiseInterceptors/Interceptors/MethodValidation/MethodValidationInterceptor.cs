using System.Linq;
using Castle.DynamicProxy;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.MethodValidation
{
    public class MethodValidationInterceptor:IInterceptor
    {
        IHelper _helper;
        IMethodValidationSettingsResolver _methodValidationSettingsResolver;

        public MethodValidationInterceptor(IMethodValidationSettingsResolver methodValidationSettingsResolver, IHelper helper)
        {
            _methodValidationSettingsResolver = methodValidationSettingsResolver;
            _helper = helper;
        }
        
        public void Intercept(IInvocation invocation)
        {
            CheckPreconditions(invocation);
            invocation.Proceed();
            CheckPostConditions(invocation);    
        }

        private void CheckPreconditions(IInvocation invocation)
        {
            if (_methodValidationSettingsResolver.BlockDefaultValueParameters(invocation.MethodInvocationTarget, invocation.Arguments))
            {
                if (invocation.Arguments.Where(p => p.Equals(_helper.GetDefaultValue(p.GetType()))).Any())
                {
                    throw new DefaultParameterValuePreConditionException(invocation, _helper);
                }
            }
        }

        private void CheckPostConditions(IInvocation invocation)
        {
            if (_methodValidationSettingsResolver.BlockDefaultResult(invocation.MethodInvocationTarget, invocation.Arguments))
            {
                if (invocation.ReturnValue.Equals(_helper.GetDefaultValue(invocation.ReturnValue.GetType())))
                {
                    throw new DefaultResultPostConditionException(invocation, _helper);
                }
            }
        }
    }
}