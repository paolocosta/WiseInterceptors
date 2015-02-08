﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;

namespace WiseInterceptors.Interceptors.CircuitBreaker
{
    
    public class CircuitBreakerInterceptor:IInterceptor
    {
        ICache _Cache;
        IHelper _helper;
        ICircuitBreakerSettingsReader _circuitBreakerSettingsReader;

        public CircuitBreakerInterceptor(ICache cache, IHelper helper, ICircuitBreakerSettingsReader circuitBreakerSettingsReader)
        {
            _Cache = cache;
            _helper = helper;
            _circuitBreakerSettingsReader = circuitBreakerSettingsReader;
        }

        public void Intercept(IInvocation invocation)
        {
            var settings = GetCircuitBreakerSettings(invocation);
            if(settings==null)
                invocation.Proceed();
            else
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

        private void HandleMethodException(IInvocation invocation, CircuitBreakerSettings settings, CircuitBreaker circuitBreaker, Exception ex)
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
            _Cache.Remove(_helper.GetMethodIdentifier(invocation));
            circuitBreaker.Status = CircuitBreakerStatusEnum.Breaked;
            circuitBreaker.BreakDate = TimeProvider.Current.UtcNow;
            _Cache.Insert(_helper.GetMethodIdentifier(invocation), circuitBreaker, TimeProvider.Current.UtcNow.AddYears(1));
        }

        private CircuitBreaker CreateNewCircuitBreaker(IInvocation invocation, CircuitBreakerSettings settings, CircuitBreaker circuitBreaker, Exception ex)
        {
            if (IsSameOrSubClassAsSettings(settings, ex))
            {
                circuitBreaker = new CircuitBreaker
                {
                    Configuration = settings,
                    CreationDate = TimeProvider.Current.UtcNow,
                    BreakingException = ex,
                    BreakDate = DateTime.MinValue,
                    Retries = 0,
                    Status = CircuitBreakerStatusEnum.Breakable
                };

                _Cache.Insert(_helper.GetMethodIdentifier(invocation),
                    circuitBreaker, TimeProvider.Current.UtcNow.AddSeconds(settings.RetryingPeriodInSeconds)
                    );
            }
            return circuitBreaker;
        }

        private static bool IsSameOrSubClassAsSettings(CircuitBreakerSettings settings, Exception ex)
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
                _Cache.Remove(_helper.GetMethodIdentifier(invocation));
            }
        }

        private void ManageExistingCircuitBreaker(IInvocation invocation, CircuitBreakerSettings settings, CircuitBreaker circuitBreaker)
        {
            if (circuitBreaker.Status == CircuitBreakerStatusEnum.Breaked && circuitBreaker.BreakDate.AddSeconds(settings.BreakingPeriodInSeconds) < TimeProvider.Current.UtcNow)
            {
                MoveToBreakableForANewTentative(invocation, settings, circuitBreaker);
            }
            if (circuitBreaker.Status == CircuitBreakerStatusEnum.Breaked)
            {
                throw new CircuitBreakerException(circuitBreaker.BreakingException);
            }
        }

        private void MoveToBreakableForANewTentative(IInvocation invocation, CircuitBreakerSettings settings, CircuitBreaker circuitBreaker)
        {
            circuitBreaker.Status = CircuitBreakerStatusEnum.Breakable;
            circuitBreaker.Retries -= 1;
            _Cache.Insert(_helper.GetMethodIdentifier(invocation),
                circuitBreaker, TimeProvider.Current.UtcNow.AddSeconds(settings.RetryingPeriodInSeconds)
                );
        }

        private CircuitBreaker GetCurrentCircuitBreaker(IInvocation invocation)
        {
            var circuitBreaker = _Cache.Get(_helper.GetMethodIdentifier(invocation)) as CircuitBreaker;
            return circuitBreaker;
        }

        private CircuitBreakerSettings GetCircuitBreakerSettings(IInvocation invocation)
        {
            return _circuitBreakerSettingsReader.GetSettings(invocation.MethodInvocationTarget, invocation.Arguments);
        }
    }
}