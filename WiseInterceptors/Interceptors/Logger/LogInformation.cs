using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseInterceptors.Interceptors.Logger
{
    public class LogInformation
    {
        public Exception RaisedException { get; set; }

        public System.Reflection.MethodBase Method { get; set; }

        public object[] Parameters { get; set; }

        public long ExecutionTime { get; set; }
    }
}
