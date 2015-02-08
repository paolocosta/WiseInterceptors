using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheDemo
{
    public class TimeGetter
    {
        public virtual DateTime Now(bool exception)
        {
            if (exception)
            {
                throw new ApplicationException("exception");
            }
            return DateTime.Now;
        }
    }
}
