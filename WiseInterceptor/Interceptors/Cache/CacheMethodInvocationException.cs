﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiseInterceptor.Interceptors.Cache
{
    internal class CacheMethodInvocationException : Exception
    {
        internal CacheMethodInvocationException(Exception ex) : base(string.Empty, ex) { }
    }
}
