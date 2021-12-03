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

            #region  Creater.LogsAuto();

            var Repository = "MeRepository";
            LogManager.CreateRepository(Repository);
            ILog logs = Log4NetExtensions.GetLog(Repository);

            logs.Debug("动态给log4net添加日志类型");
            logs.Info("动态生成日志配置项  生成的日志配应该是保存在内存中的，如果停止运行会消失，不会保存到log4net.config文件中");
            logs.Warn("WarnWarnWarnWarnWarnWarn"); 

            #endregion



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
