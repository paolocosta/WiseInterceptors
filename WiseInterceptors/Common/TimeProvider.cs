﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Common
{
    public abstract class TimeProvider
    {
        private static TimeProvider current =
        DefaultTimeProvider.Instance;

        public static TimeProvider Current
        {
            get { return TimeProvider.current; }
            set
            {
                TimeProvider.current = value;
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
            get { return DefaultTimeProvider.instance; }
        }
    }
}