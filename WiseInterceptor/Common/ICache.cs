using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Common
{
    public interface ICache
    {
        void Insert(string Key, object Value, DateTime Expiration);

        object Get(string Key);

        void Remove(string Key);

        void InsertInPersistantCache(string Key, object Value);

        object GetFromPersistantCache(string Key);
    }
}
