﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.CircuitBreaker
{
    public interface ICircuitBreakerSettingsReader
    {
        CircuitBreakerSettings GetSettings(MethodInfo method, object[] arguments);
    }
}
