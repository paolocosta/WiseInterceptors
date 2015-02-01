using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Common;

namespace WiseInterceptors.Test
{
    public class CustomTimeProvider:TimeProvider
    {
        private DateTime _now;
        public override DateTime UtcNow
        {
            get {
                return _now;
            }
        }

        public override void SetCurrentTime(DateTime time)
        {
            _now = time;
        }
    }
}
