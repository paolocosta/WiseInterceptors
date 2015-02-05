﻿using System;
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

    public enum FaultToleranceEnum
    {
        AlwaysUsePersistentCache,
        UsePersistentCacheOnlyInCaseOfError,
        ProlongCacheInCaseOfError,
        FailFast
    }

    public class CacheSettings
    {
        public CacheSettings()
        {
            Duration = 20 * 60;
            Priority = PriorityEnum.Normal;
        }

        public int Duration { get; set; }
        public PriorityEnum Priority { get; set; }
        public FaultToleranceEnum FaultToleranceType { get; set; }
        public bool UseCache { get; set; }
    }

}
