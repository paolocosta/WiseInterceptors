﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor;
using WiseInterceptor.Common;

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

        public void InsertInPersistantCache(string Key, object Value)
        {
            throw new NotImplementedException();
        }


        public void GetFromPersistantCache(string Key)
        {
            throw new NotImplementedException();
        }


        object ICache.GetFromPersistantCache(string Key)
        {
            throw new NotImplementedException();
        }


        public WiseInterceptor.Interceptors.Cache.CacheSettings GetSettings(System.Reflection.MethodInfo method, object[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
