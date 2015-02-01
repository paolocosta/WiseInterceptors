using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Common;

namespace WiseInterceptor.Interceptors.CircuitBreaker
{
    
    public class CircuitBreakerInterceptor:IInterceptor
    {
        ICache _Cache;
        IHelper _Helper;

        public CircuitBreakerInterceptor(ICache cache)
        {
            _Cache = cache;
            _Helper = new Helper();
        }

        public void Intercept(IInvocation invocation)
        {
            var settings = GetCircuitBreakerSettings(invocation);
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

                    //In case of succesful call we can close the circuit (= delete the circuit breaker)
                    CloseCircuit(invocation, circuitBreaker);
                }
                catch (CircuitBreakerException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    HandleMethodException(invocation, settings, circuitBreaker, ex);
                    throw;
                }
            }
        }

        private void HandleMethodException(IInvocation invocation, CircuitBreakerSettingsAttribute settings, CircuitBreaker circuitBreaker, Exception ex)
        {
            if (circuitBreaker == null)
            {
                circuitBreaker = CreateNewCircuitBreaker(invocation, settings, circuitBreaker, ex);
            }

            circuitBreaker.Retries++;

            if (circuitBreaker.Retries >= settings.ExceptionsBeforeBreak)
            {
                BreakCircuit(invocation, circuitBreaker);
            }
        }

        private void BreakCircuit(IInvocation invocation, CircuitBreaker circuitBreaker)
        {
            _Cache.Remove(_Helper.GetMethodIdentifier(invocation));
            circuitBreaker.Status = CircuitBreakerStatusEnum.Breaked;
            circuitBreaker.BreakDate = _Cache.Now();
            _Cache.Insert(_Helper.GetMethodIdentifier(invocation), circuitBreaker, _Cache.Now().AddYears(1));
        }

        private CircuitBreaker CreateNewCircuitBreaker(IInvocation invocation, CircuitBreakerSettingsAttribute settings, CircuitBreaker circuitBreaker, Exception ex)
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

                _Cache.Insert(_Helper.GetMethodIdentifier(invocation),
                    circuitBreaker, _Cache.Now().AddSeconds(settings.RetryingPeriodInSeconds)
                    );
            }
            return circuitBreaker;
        }

        private static bool IsSameOrSubClassAsSettings(CircuitBreakerSettingsAttribute settings, Exception ex)
        {
            return ex.GetType().IsSubclassOf(settings.ExceptionType) || ex.GetType() == settings.ExceptionType;
        }

        /// <summary>
        /// This method closes the circuit, so energy can flow normally
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="circuitBreaker"></param>
        private void CloseCircuit(IInvocation invocation, CircuitBreaker circuitBreaker)
        {
            if (circuitBreaker != null)
            {
                _Cache.Remove(_Helper.GetMethodIdentifier(invocation));
            }
        }

        private void ManageExistingCircuitBreaker(IInvocation invocation, CircuitBreakerSettingsAttribute settings, CircuitBreaker circuitBreaker)
        {
            if (circuitBreaker.Status == CircuitBreakerStatusEnum.Breaked && circuitBreaker.BreakDate.AddSeconds(settings.BreakingPeriodInSeconds) < _Cache.Now())
            {
                MoveToBreakableForANewTentative(invocation, settings, circuitBreaker);
            }
            if (circuitBreaker.Status == CircuitBreakerStatusEnum.Breaked)
            {
                throw new CircuitBreakerException(circuitBreaker.BreakingException);
            }
        }

        private void MoveToBreakableForANewTentative(IInvocation invocation, CircuitBreakerSettingsAttribute settings, CircuitBreaker circuitBreaker)
        {
            circuitBreaker.Status = CircuitBreakerStatusEnum.Breakable;
            circuitBreaker.Retries -= 1;
            _Cache.Insert(_Helper.GetMethodIdentifier(invocation),
                circuitBreaker, _Cache.Now().AddSeconds(settings.RetryingPeriodInSeconds)
                );
        }

        private CircuitBreaker GetCurrentCircuitBreaker(IInvocation invocation)
        {
            var circuitBreaker = _Cache.Get(_Helper.GetMethodIdentifier(invocation)) as CircuitBreaker;
            return circuitBreaker;
        }

        private static CircuitBreakerSettingsAttribute GetCircuitBreakerSettings(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            var settings = methodInfo.GetCustomAttributes(typeof(CircuitBreakerSettingsAttribute), true).SingleOrDefault()
                as CircuitBreakerSettingsAttribute;
            return settings;
        }
    }
}
