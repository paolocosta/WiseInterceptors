using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Common;

namespace CacheDemo
{
    public class Cache:ICache
    {
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
            throw new NotImplementedException();
        }

        public object GetFromPersistentCache(string Key)
        {
            throw new NotImplementedException();
        }

        public WiseInterceptors.Interceptors.Cache.CacheSettings GetSettings(System.Reflection.MethodInfo method, object[] arguments)
        {
            return Program._Form1.GetCacheSettings();
        }
    }
}
