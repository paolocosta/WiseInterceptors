using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Interceptors.Cache
{
    public class CacheValue
    {
        public object Value { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Persisted { get; set; }
    }
}
