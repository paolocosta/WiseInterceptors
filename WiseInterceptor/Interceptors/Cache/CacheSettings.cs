using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptor.Interceptors.Cache
{
    public enum PriorityEnum
    { 
        Low,
        Normal,
        High
    }

    public class CacheSettings
    {
        public CacheSettings()
        {
            Duration = 20;
            Priority = PriorityEnum.Normal;
            SoftDuration = 10; 
        }

        public int Duration { get; set; }
        public int SoftDuration { get; set; }
        public PriorityEnum Priority { get; set; }
    }

}
