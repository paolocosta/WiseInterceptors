namespace WiseInterceptors.Interceptors.Cache
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
        ConsiderSoftlyExpiredValuesInCaseOfErrors,
        FailFastWithNoRecovery
    }

    public class CacheSettings
    {
        public int Duration { get; set; }
        public PriorityEnum Priority { get; set; }
        public bool UseCache { get; set; }
        public string Key { get; set; }
    }
}
