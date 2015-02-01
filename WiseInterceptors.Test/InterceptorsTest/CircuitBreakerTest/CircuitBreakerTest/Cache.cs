using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor;
using WiseInterceptor.Common;

namespace WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest
{
    internal class CacheStub : ICache
    {
        Dictionary<string, Tuple<object, DateTime>> _Elements = new Dictionary<string, Tuple<object, DateTime>>();

        public void Insert(string Key, object Value, DateTime Expiration)
        {
            if (_Elements.ContainsKey(Key))
                _Elements.Remove(Key);
            _Elements.Add( Key, Tuple.Create(Value, Expiration));
        }

        public object Get(string Key)
        {
            if (_Elements.ContainsKey(Key) && TimeProvider.Current.UtcNow < _Elements[Key].Item2)
            {
                return _Elements[Key].Item1;
            }
            return null;
        }

        public void Remove(string Key)
        {
            _Elements.Remove(Key);
        }

        public void Reset()
        {
            _Elements = new Dictionary<string, Tuple<object, DateTime>>();
        }

    }
}
