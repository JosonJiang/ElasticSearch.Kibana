using log4net;
using log4net.Config;
using NET.Standard.Elastic.Kibana.Log4Net;
using System;
using System.IO;
using System.Reflection;

namespace Joson.ElasticSearch.Kibana.ConsolesApp
{
    class Program
    {







        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var log4netRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(log4netRepository, new FileInfo("log4net.config"));
            ILog log = LogManager.GetLogger(log4netRepository.Name, "NETCorelog4net");

            log.Info("NETCorelog4net log");
            log.Error("Error log");
            log.Debug("Debug");
            log.Warn("Warn");
 
            try
            {
                log.Info(new LogContent("127.0.0.1", "111111", "登陆系统", "登陆成功"));
                var ss = 1 - int.Parse("sss");
            }
            catch (Exception ex)
            {
                log.Error(new LogContent("127.0.0.1", "111111", "登陆系统", ex.Message + ":" + ex.StackTrace));
            }

            //Creater.Logs();

            Console.ReadLine();
        }
    }
}
