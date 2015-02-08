﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Interceptors.CircuitBreaker
{
    internal enum CircuitBreakerStatusEnum 
    {
        Off,
        Breakable,
        Breaked
    }

    internal class CircuitBreaker
    {
        internal CircuitBreakerSettings Configuration { get; set; }
        internal DateTime CreationDate { get; set; }
        internal DateTime BreakDate { get; set; }
        internal int Retries { get; set; }
        internal CircuitBreakerStatusEnum Status { get; set; }
        internal Exception BreakingException { get; set; }
    }
}