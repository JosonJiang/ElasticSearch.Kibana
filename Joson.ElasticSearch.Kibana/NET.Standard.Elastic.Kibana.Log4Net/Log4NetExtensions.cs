using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NET.Standard.Elastic.Kibana.Log4Net
{
    public static class Log4NetExtensions
    {

        #region 动态给log4net添加日志类型
        private static object m_Lock = new object();

        /// <summary>
        /// 根据类型获取对应的日志操作类
        /// 这里在初始化时要先创建Repository，如：LogManager.CreateRepository("MeIsTestRepository");
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static ILog GetLog(String IRepository, string typeName = "JosonLogAppender")
        {
            lock (m_Lock)
            {
                //这里在初始化时要先创建Repository，如：LogManager.CreateRepository("MeRepository");
                //创建之后才能在这里获取到 
                //https://www.cnblogs.com/qiywtc/p/9837506.html

                var Repository = LogManager.GetRepository(IRepository);

                var log = LogManager.Exists(IRepository, typeName);
                if (log != null)
                {
                    return log;
                }

                CreateXMLLog(Repository, typeName);
                return LogManager.GetLogger(IRepository, typeName);
            }
        }

        /// <summary>
        /// 动态添加日志类型
        /// </summary>
        /// <param name="logRepository"></param>
        /// <param name="typeName"></param>
        private static void CreateXMLLog(log4net.Repository.ILoggerRepository logRepository, string typeName)
        {


           //<?xml version = "1.0" encoding="UTF-8"?>
           // <configuration>
           // <log4net>
           //     <appender name = "JosonLogAppenderAppender" type="log4net.Appender.RollingFileAppender">
           //         <param name = "Encoding" value="utf-8" />
           //         <file value = "Logs/" />
           //         <appendToFile value="true" />
           //         <rollingStyle value = "Composite" />
           //         <staticLogFileName value="false" />
           //         <DatePattern value = "yyyy/yyyy-MM/'JosonLogAppender'-yyyy-MM-dd.log" />
           //         <maxSizeRollBackups value="10" />

           //         <maximumFileSize value = "1MB" />
           //             <layout type="log4net.Layout.PatternLayout">
           //             <conversionPattern value = "$$%date%message%newline" />
           //         </layout>

           //     </appender>
           //     <logger name="JosonLogAppender">
           //         <level value = "DEBUG" />
           //         <appender - ref ref= "JosonLogAppenderAppender" />
           //     </logger >
           // </log4net>
           // </configuration>



            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            var configuration = xmlDoc.CreateElement("configuration");
            var root = xmlDoc.CreateElement("log4net");

            var appender = xmlDoc.CreateElement("appender");
            appender.SetAttribute("name", typeName + "Appender");
            appender.SetAttribute("type", "log4net.Appender.RollingFileAppender");
            var param = xmlDoc.CreateElement("param");
            param.SetAttribute("name", "Encoding");
            param.SetAttribute("value", "utf-8");
            appender.AppendChild(param);

            var file = xmlDoc.CreateElement("file");
            file.SetAttribute("value", "Logs/");
            appender.AppendChild(file);

            var appendToFile = xmlDoc.CreateElement("appendToFile");
            appendToFile.SetAttribute("value", "true");
            appender.AppendChild(appendToFile);

            var rollingStyle = xmlDoc.CreateElement("rollingStyle");
            rollingStyle.SetAttribute("value", "Composite");
            appender.AppendChild(rollingStyle);

            var staticLogFileName = xmlDoc.CreateElement("staticLogFileName");
            staticLogFileName.SetAttribute("value", "false");
            appender.AppendChild(staticLogFileName);

            var DatePattern = xmlDoc.CreateElement("DatePattern");
            DatePattern.SetAttribute("value", $"yyyy/yyyy-MM/yyyy-MM-dd.'{typeName}.log'");
            //DatePattern.SetAttribute("value", $"yyyy/yyyy-MM/dd-'{typeName}.log'");
            appender.AppendChild(DatePattern);


            var maxSizeRollBackups = xmlDoc.CreateElement("maxSizeRollBackups");
            maxSizeRollBackups.SetAttribute("value", "10");
            appender.AppendChild(maxSizeRollBackups);

            var maximumFileSize = xmlDoc.CreateElement("maximumFileSize");
            maximumFileSize.SetAttribute("value", "1MB");
            appender.AppendChild(maximumFileSize);

            var layout = xmlDoc.CreateElement("layout");
            layout.SetAttribute("type", "log4net.Layout.PatternLayout");

            var conversionPattern = xmlDoc.CreateElement("conversionPattern");
            conversionPattern.SetAttribute("value", "%date%message%newline");
            layout.AppendChild(conversionPattern);
            appender.AppendChild(layout);

            var logger = xmlDoc.CreateElement("logger");
            logger.SetAttribute("name", typeName);
            var level = xmlDoc.CreateElement("level");
            level.SetAttribute("value", "DEBUG");
            var appender_ref = xmlDoc.CreateElement("appender-ref");
            appender_ref.SetAttribute("ref", typeName + "Appender");
            logger.AppendChild(level);
            logger.AppendChild(appender_ref);


            root.AppendChild(appender);
            root.AppendChild(logger);

            configuration.AppendChild(root);
            xmlDoc.AppendChild(configuration);

            var array = System.Text.Encoding.ASCII.GetBytes(xmlDoc.OuterXml);
            var stream = new MemoryStream(array);
            var reader = new StreamReader(stream);

            log4net.Config.XmlConfigurator.Configure(logRepository, reader.BaseStream);

        }


        #region 动态生成日志配置项  生成的日志配应该是保存在内存中的，如果停止运行会消失，不会保存到log4net.config文件中

        //<logger name = "JosonLog">
        //  <level value="DEBUG"/>
        //  <appender-ref ref="JosonLogAppender" />
        //</logger>

        //<appender name = "JosonLogAppender" type="log4net.Appender.RollingFileAppender">
        //  <file value = "logfile/" />
        //  <appendToFile value="true" />
        //  <rollingStyle value = "Composite" />
        //  <staticLogFileName value="false" />
        //  <datePattern value = "yyyy/yyyy-MM/TestLog-yyyy-MM-dd.TXT" /> 
        //  <maxSizeRollBackups value="10" /> 
        //  <maximumFileSize value = "1MB" />
        //  <layout type="log4net.Layout.PatternLayout"> 
        //  <conversionPattern value = "%message%newline" /> </layout> 
        // </appender > 

        #endregion

        #endregion

    }
}
