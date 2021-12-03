using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Net.Monitor.Helper
{
    public class LogEntity
    {

        public int? MemberID { get; set; }
        public String UserNick { get; set; }
        public string ServiceName { get; set; }
        public string Action { get; set; }
        public string Method { get; set; }
        public string Parameters { get; set; }
        public string ClientIP { get; set; }
        public string ClientName { get; set; }
        public string BrowserInfo { get; set; }
        public string OperatingAddress { get; set; }
        public string OS { get; set; }
        public string TraceID { get; set; }
        public string EventID { get; set; }
        public string Category { get; set; }
        
        public string Exception { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; internal set; }
        public Object CustomData { get; set; }
        public long ExecutionDuration { get; internal set; }

 

    }
}