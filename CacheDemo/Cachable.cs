using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheDemo
{
    public class Cachable : CacheDemo.ICachable
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
