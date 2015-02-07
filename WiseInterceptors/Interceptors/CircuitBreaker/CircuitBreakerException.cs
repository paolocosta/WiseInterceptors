using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Interceptors.CircuitBreaker
{
    public class CircuitBreakerException:Exception
    {
        public CircuitBreakerException(Exception ex)
            : base(ex.Message, ex)
        { }
    }
}
