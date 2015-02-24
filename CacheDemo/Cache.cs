using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Caching;
using WiseInterceptors.Common;
using WiseInterceptors.Interceptors.Cache;

namespace CacheDemo
{
    public class Cache:ICache
    {
        private static Dictionary<string, object> _PersistantCache = new Dictionary<string, object>();


        public void Insert(string Key, object Value, DateTime Expiration)
        {
            Remove(Key);
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

        public void InsertInPersistentCache(string Key, object Value)
        {
            if (_PersistantCache.ContainsKey(Key))
                _PersistantCache.Remove(Key);

            _PersistantCache.Add(Key, Value);
        }

        public object GetFromPersistentCache(string Key)
        {
            if (_PersistantCache.ContainsKey(Key))
                return _PersistantCache[Key];

            return null;
        }

        public CacheSettings GetSettings(MethodInfo method, object[] arguments)
        {
            return Program._Form1.GetCacheSettings();
        }


        public FaultToleranceEnum GetFaultToleranceStrategy()
        {
            return Program._Form1.GetFaultTolerance();
        }
    }
}
