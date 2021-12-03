using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Web;



namespace NET.Standard.Elastic.Kibana.Log4Net
{
    using log4net;
    using log4net.Config;
    using log4net.Layout;
    using log4net.Layout.Pattern;

    //Apache log4net™ Config Examples
    //http://logging.apache.org/log4net/release/config-examples.html

    //https://csharp.hotexamples.com/examples/log4net.Layout/PatternLayout/ActivateOptions/php-patternlayout-activateoptions-method-examples.html



    //Log4Net, how to add a custom field to my logging
    //https://stackoverflow.com/questions/12139486/log4net-how-to-add-a-custom-field-to-my-logging
    //CustomerPatternConverter
    //https://blog.csdn.net/kongwei521/article/details/52242319

    //https://stackoverflow.com/questions/10834884/log4net-with-adonetappender-nothing-happens
    //https://hendrikbulens.com/2016/01/14/custom-properties-in-log4net-logging/


    //ELK日志框架（2）：log4net.ElasticSearch+ Kibana实现日志记录和显示
    //https://www.cnblogs.com/zeroes/p/elk-log4net-kibana.html





    //https://thesoftwayfarecoder.com/including-the-user-name-info-in-the-conversion-pattern-of-log4net-on-asp-net-core/
    //https://codedefault.com/p/log4net-extension-aspnetcore
    //.NET 结合ELK+log4net实现集中式日志解决方案
    //https://www.zhidao91.com/donet-elk-log4net/
    public static class Creater
    {

        //public  Creater()
        //{

        //}



        public static ILog LogsAuto(String Repository = "MeIRepository")
        {

            log4net.LogManager.CreateRepository(Repository);
            log4net.ILog logs = Log4Net.Log4NetExtensions.GetLog(Repository);
            logs.Debug("LogsAuto 动态给log4net添加日志类型");
            logs.Info(" LogsAuto 动态生成日志配置项  生成的日志配应该是保存在内存中的，如果停止运行会消失，不会保存到log4net.config文件中");

            return logs;

        }


        public static ILog Logs(String ConfigFile = "log4net.config", String LoggerName = "NETCorelog4net")
        {

            var log4netRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(log4netRepository, new FileInfo(ConfigFile));

            ILog log = LogManager.GetLogger(log4netRepository.Name, LoggerName);

            log.Info("NETCorelog4net log");
            log.Info("test log");
            log.Error("error");
            log.Info("linezero");
            return log;

        }

    }


    /// <summary>
    /// 包含了所有的自定字段属性
    /// </summary>
    public class LogContent
    {
        public LogContent(string macAddress, string computerName, string actionsclick, string description)
        {
            UserIP = macAddress;
            UserName = computerName;
            ActionsClick = actionsclick;
            Message = description;
        }

        /// <summary>
        /// 访问IP
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 系统登陆用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 动作事件
        /// </summary>
        public string ActionsClick { get; set; }

        /// <summary>
        /// 日志描述信息
        /// </summary>
        public string Message { get; set; }


    }

}
