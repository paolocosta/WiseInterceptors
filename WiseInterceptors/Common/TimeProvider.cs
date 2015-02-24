using System;

namespace WiseInterceptors.Common
{
    public abstract class TimeProvider
    {
        private static TimeProvider current =
        DefaultTimeProvider.Instance;

        public static TimeProvider Current
        {
            get { return current; }
            set
            {
                current = value;
            }
        }

        public static void ResetDefault()
        {
            current = DefaultTimeProvider.Instance;
        }

        public abstract DateTime UtcNow { get; }
    }

    public class DefaultTimeProvider : TimeProvider
    {
        private readonly static DefaultTimeProvider instance =
           new DefaultTimeProvider();

        private DefaultTimeProvider() { }

        public override DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public static DefaultTimeProvider Instance
        {
            get { return instance; }
        }
    }
}
