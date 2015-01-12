using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor;

namespace WiseInterceptors.Test.InterceptorsTest.CircuitBreakerTest.CircuitBreakerTest
{
    internal class CacheStub : ICache
    {
        public DateTime FakeNow { get; set; }

        Dictionary<string, Tuple<object, DateTime>> _Elements = new Dictionary<string, Tuple<object, DateTime>>();

        public void Insert(string Key, object Value, DateTime Expiration)
        {
            if (_Elements.ContainsKey(Key))
                _Elements.Remove(Key);
            _Elements.Add( Key, Tuple.Create(Value, Expiration));
        }

        public object Get(string Key)
        {
            if(_Elements.ContainsKey(Key) && Now() < _Elements[Key].Item2)
            {
                return _Elements[Key].Item1;
            }
            return null;
        }

        public void Remove(string Key)
        {
            _Elements.Remove(Key);
        }

        public DateTime Now()
        {
            return FakeNow;
        }

        public void Reset()
        {
            _Elements = new Dictionary<string, Tuple<object, DateTime>>();
        }

    }
}
