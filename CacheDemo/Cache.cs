using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor;

namespace CacheDemo
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
    }
}
