using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiseInterceptors.Interceptors.Cache
{
    public class CacheMethodInvocationException : Exception
    {
        public CacheMethodInvocationException(Exception ex) : base(string.Empty, ex) { }
    }
}
