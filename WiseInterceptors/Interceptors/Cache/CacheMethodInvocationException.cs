using System;

namespace WiseInterceptors.Interceptors.Cache
{
    public class CacheMethodInvocationException : Exception
    {
        public CacheMethodInvocationException(Exception ex) : base(string.Empty, ex) { }
    }
}
