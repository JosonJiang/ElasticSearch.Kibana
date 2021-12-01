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
    using MicroKnights.Logging;

    //http://logging.apache.org/log4net/release/config-examples.html

    //https://blog.csdn.net/kongwei521/article/details/52242319
    //https://www.cnblogs.com/zeroes/p/elk-log4net-kibana.html
    //https://stackoverflow.com/questions/10834884/log4net-with-adonetappender-nothing-happens

    //.NET 结合ELK+log4net实现集中式日志解决方案
    //https://www.zhidao91.com/donet-elk-log4net/
    public class Creater
    {

        public Creater()
        {

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


    #region JosonLayout

    public class JosonLayout : PatternLayout
    {
        public JosonLayout()
        {
            this.AddConverter("property", typeof(LogInfoPatternConverter));
        }
    }

    public class LogInfoPatternConverter : PatternLayoutConverter
    {

        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                // Write the value for the specified key
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }
        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>

        private object LookupProperty(string property, log4net.Core.LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }
    }

    #endregion


    #region 数据库日志相关
    /// <summary>
    /// 将数据库链接字符串独立出来
    /// </summary>
    public class MyAdoNetAppender : AdoNetAppender
    {
        //private static IJosonLog _Log;
        //private string _ConnectionStringName;
        //public string ConnectionStringName
        //{
        //    get { return _ConnectionStringName; }
        //    set { _ConnectionStringName = value; }
        //}
        //protected static IJosonLog Log
        //{
        //    get
        //    {
        //        if (_Log == null)
        //            _Log = JosonLogManager.GetLogger(typeof(MyAdoNetAppender));
        //        return _Log;
        //    }
        //}
        //public override void ActivateOptions() { PopulateConnectionString(); base.ActivateOptions(); }

        ///// <summary>
        ///// 获取或设置数据库连接字符串
        ///// </summary>
        //private void PopulateConnectionString()
        //{
        //    // 如果配置文件中设置了ConnectionString，则返回
        //    if (!String.IsNullOrEmpty(ConnectionString)) return;
        //    // 如果配置文件中没有设置ConnectionStringName，则返回
        //    if (String.IsNullOrEmpty(ConnectionStringName)) return;
        //    // 获取对应Web.config中的连接字符串配置
        //    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
        //    if (settings == null)
        //    {
        //        if (Log.IsErrorEnabled)
        //            Log.ErrorFormat("Connection String Name not found in Configuration: {0}", ConnectionStringName);
        //        return;
        //    }
        //    //返回解密的连接字符串
        //    ConnectionString = settings.ConnectionString;// new DESEncrypt().Decrypt(settings.ConnectionString, null);
        //}
    }
    #endregion


}
