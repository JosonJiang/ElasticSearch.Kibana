using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Joson.ElasticSearch.Kibana.ConsoleApps
{
    using log4net;
   // using NET.Standard.Elastic.Kibana.Log4Net;
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            _log.Error("kaboom!", new ApplicationException("The application exploded"));
        }
    }
}
