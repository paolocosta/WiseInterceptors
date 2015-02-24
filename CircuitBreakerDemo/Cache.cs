using System;
using System.Reflection;
using System.Runtime.Caching;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache;

namespace CircuitBreakerDemo
{
    internal class Cache : ICache
    {
        public void Insert(string Key, object Value, DateTime Expiration)
        {
            MemoryCache.Default.Add(Key, Value, Expiration);
        }

        public object Get(string Key)
        {
            return MemoryCache.Default.Get(Key);
        }

        public void Remove(string Key)
        {
            MemoryCache.Default.Remove(Key);
        }

        public DateTime Now()
        {
            return DateTime.Now;
        }

        public void InsertInPersistentCache(string Key, object Value)
        {
            throw new NotImplementedException();
        }

        public void GetFromPersistentCache(string Key)
        {
            throw new NotImplementedException();
        }

        object ICache.GetFromPersistentCache(string Key)
        {
            throw new NotImplementedException();
        }

        public CacheSettings GetSettings(MethodInfo method, object[] arguments)
        {
            throw new NotImplementedException();
        }


        public FaultToleranceEnum GetFaultToleranceStrategy()
        {
            throw new NotImplementedException();
        }
    }
}
