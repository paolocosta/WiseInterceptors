using System;

namespace WiseInterceptors.Interceptors.Cache
{
    public class CacheValue
    {
        public object Value { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Persisted { get; set; }
    }
}
