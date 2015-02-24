using System;
using System.Reflection;

namespace WiseInterceptors.Interceptors.Logger
{
    public class LogInformation
    {
        public Exception RaisedException { get; set; }

        public MethodBase Method { get; set; }

        public object[] Parameters { get; set; }

        public long ExecutionTime { get; set; }
    }
}
