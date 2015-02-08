using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptors.Interceptors.Cache;

namespace WiseInterceptors.Common
{
    public interface ICache
    {
        void Insert(string Key, object Value, DateTime Expiration);

        object Get(string Key);

        void Remove(string Key);

        void InsertInPersistentCache(string Key, object Value);

        object GetFromPersistentCache(string Key);

        CacheSettings GetSettings(MethodInfo method, object[] arguments);
    }
}
