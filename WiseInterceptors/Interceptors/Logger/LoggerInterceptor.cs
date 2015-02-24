using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace WiseInterceptors.Interceptors.Logger
{
    public class LoggerInterceptor : IInterceptor
    {
        private ILogger _logger;

        public LoggerInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                invocation.Proceed();
                sw.Stop();
                if (_logger.GetLogSettings(invocation.MethodInvocationTarget, invocation.Arguments).Logcall)
                    _logger.Log(new LogInformation { Method = invocation.MethodInvocationTarget, Parameters = invocation.Arguments, ExecutionTime = sw.ElapsedMilliseconds });
            }
            catch (Exception ex)
            {
                sw.Stop();
                if (_logger.GetLogSettings(invocation.MethodInvocationTarget, invocation.Arguments).LogException)
                    _logger.Log(new LogInformation { Method = invocation.MethodInvocationTarget, Parameters = invocation.Arguments, RaisedException = ex, ExecutionTime = sw.ElapsedMilliseconds });
            }
        }
    }
}
