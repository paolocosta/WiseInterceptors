using System;

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
