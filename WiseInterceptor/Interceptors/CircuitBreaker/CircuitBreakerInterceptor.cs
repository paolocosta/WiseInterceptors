using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.CircuitBreaker
{
    
    public class CircuitBreakerInterceptor:IInterceptor
    {
        ICache _Cache;
        public CircuitBreakerInterceptor(ICache cache)
        {
            _Cache = cache;
        }

        public void Intercept(IInvocation invocation)
        {
            var settings = GetMethodSettings(invocation);
            if (settings != null)
            {

                var circuitBreaker = GetCurrentCircuitBreaker(invocation);
                try
                {
                    if (circuitBreaker != null)
                    {
                        ManageExistingCircuitBreaker(invocation, settings, circuitBreaker);
                    }
                    invocation.Proceed();

                    RemoveCircuitBreaker(invocation, circuitBreaker);
                }
                catch (Exception ex)
                {
                    RethrowCircuitBreakerException(ex);
                    
                    if (circuitBreaker == null)
                    {
                        if (IsSameOrSubClassAsSettings(settings, ex))
                        {
                            circuitBreaker = new CircuitBreaker
                            {
                                Configuration = settings,
                                CreationDate = _Cache.Now(),
                                BreakingException = ex,
                                BreakDate = DateTime.MinValue,
                                Retries = 0,
                                Status = CircuitBreakerStatusEnum.Breakable
                            };

                            _Cache.Insert(GetCacheKey(invocation),
                                circuitBreaker, _Cache.Now().AddSeconds(settings.RetryingPeriodInSeconds)
                                );
                        }
                    }
                    
                    circuitBreaker.Retries++;
                    
                    if (circuitBreaker.Retries >= settings.ExceptionsBeforeBreak)
                    {
                        _Cache.Remove(GetCacheKey(invocation));
                        circuitBreaker.Status = CircuitBreakerStatusEnum.Breaked;
                        circuitBreaker.BreakDate = _Cache.Now();
                        _Cache.Insert(GetCacheKey(invocation), circuitBreaker, _Cache.Now().AddYears(1));
                    }
                    throw ex;
                }
            }
        }

        private static bool IsSameOrSubClassAsSettings(CircuitBreakerSettingsAttribute settings, Exception ex)
        {
            return ex.GetType().IsSubclassOf(settings.ExceptionType) || ex.GetType() == settings.ExceptionType;
        }

        private static void RethrowCircuitBreakerException(Exception ex)
        {
            if (ex.GetType() == typeof(CircuitBreakerException))
            {
                throw ex;
            }
        }

        private void RemoveCircuitBreaker(IInvocation invocation, CircuitBreaker circuitBreaker)
        {
            if (circuitBreaker != null)
            {
                _Cache.Remove(GetCacheKey(invocation));
            }
        }

        private void ManageExistingCircuitBreaker(IInvocation invocation, CircuitBreakerSettingsAttribute settings, CircuitBreaker circuitBreaker)
        {
            if (circuitBreaker.Status == CircuitBreakerStatusEnum.Breaked && circuitBreaker.BreakDate.AddSeconds(settings.BreakingPeriodInSeconds) > _Cache.Now())
            {
                circuitBreaker.Status = CircuitBreakerStatusEnum.Breakable;
                circuitBreaker.Retries -= 1;
                _Cache.Insert(GetCacheKey(invocation),
                    circuitBreaker, _Cache.Now().AddSeconds(settings.RetryingPeriodInSeconds)
                    );
            }
            if (circuitBreaker.Status == CircuitBreakerStatusEnum.Breaked)
            {
                throw new CircuitBreakerException(circuitBreaker.BreakingException);
            }
        }

        private CircuitBreaker GetCurrentCircuitBreaker(IInvocation invocation)
        {
            var circuitBreaker = _Cache.Get(GetCacheKey(invocation)) as CircuitBreaker;
            return circuitBreaker;
        }

        private static CircuitBreakerSettingsAttribute GetMethodSettings(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }
            var settings = methodInfo.GetCustomAttributes(typeof(CircuitBreakerSettingsAttribute), true).SingleOrDefault()
                as CircuitBreakerSettingsAttribute;
            return settings;
        }

        private string GetCacheKey(IInvocation invocation)
        {
            StringBuilder cacheKey = new StringBuilder();
            cacheKey.Append(string.Format("{0}_", invocation.Method.Name));
            //if (_Settings.VaryByMethodArgs != null)
            //{
            //    var args = _Settings.VaryByMethodArgs.Split(',');
            //    foreach (var arg in args)
            //    {
            //        //cacheKey.Append(string.Format("{0}_", Newtonsoft.Json.JsonConvert.SerializeObject(invocation.Arguments;
            //    }
            //}

            return cacheKey.ToString();
        }
    }
}
