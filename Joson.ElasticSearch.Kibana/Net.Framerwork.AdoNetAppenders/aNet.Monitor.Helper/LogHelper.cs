using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Web;

namespace Net.Monitor.Helper
{


    public class LogHelper
    {
        #region LogManager

        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        private static readonly log4net.ILog logmonitor = log4net.LogManager.GetLogger("logmonitor");

        public static void Error(string ErrorMsg, Exception ex = null)
        {
            if (ex != null)
            {
                logerror.Error(ErrorMsg, ex);
            }
            else
            {
                logerror.Error(ErrorMsg);
            }
        }

        public static void Info(string Msg)
        {
            loginfo.Info(Msg);
        }

        public static void Monitor(string Msg)
        {
            logmonitor.Info(Msg);
        }

        #endregion


        #region WriteLogEntity

        public static string ConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + @"/Properties/log4net.config";
        public static LogHelper logHelper = new LogHelper();
        public LogHelper()
        {
            XmlConfigurator.Configure();
        }

        public bool Write(LogEntity logInfo)
        {
            //自定义添加的属性
            string propertiesMemberId = "MemberID";
            string propertiesUserNick = "UserNick";
            string propertiesServiceName = "ServiceName";
            string propertiesAction = "Action";
            string propertiesMethod = "Method";
            string propertiesParameters = "Parameters";
            string propertiesClientIpAddress = "ClientIP";
            string propertiesClientName = "ClientName";
            string propertiesBrowserInfo = "BrowserInfo";
            string propertiesException = "Exception";
            string propertiesExceptionMessage = "ExceptionMessage";
            string propertiesStacktrace = "StackTrace";
            string propertiesCustomData = "CustomData";
            string propertiesExecutionDuration = "ExecutionDuration";

            string propertiesOperatingAddress = "OperatingAddress";
            string propertiesOS = "OS";
            string propertiesTraceID = "TraceID";
            string propertiesEventID = "EventID";


            string pathlog4net = ConfigFilePath;
            XmlConfigurator.Configure(new FileInfo(pathlog4net));
            try
            {

                GlobalContext.Properties[propertiesUserNick] = logInfo.UserNick;
                GlobalContext.Properties[propertiesMemberId] = logInfo.MemberID;
                GlobalContext.Properties[propertiesServiceName] = logInfo.ServiceName;
                GlobalContext.Properties[propertiesAction] = logInfo.Action;
                GlobalContext.Properties[propertiesMethod] = logInfo.Method;
                GlobalContext.Properties[propertiesParameters] = logInfo.Parameters;
                GlobalContext.Properties[propertiesClientIpAddress] = logInfo.ClientIP;
                GlobalContext.Properties[propertiesClientName] = logInfo.ClientName;
                GlobalContext.Properties[propertiesBrowserInfo] = logInfo.BrowserInfo;
                GlobalContext.Properties[propertiesCustomData] = logInfo.CustomData;
                GlobalContext.Properties[propertiesException] = logInfo.Exception == null ? "" : logInfo.Exception;
                GlobalContext.Properties[propertiesExceptionMessage] = logInfo.ExceptionMessage == null ? "" : logInfo.ExceptionMessage;


                GlobalContext.Properties["StackTrace"] = logInfo.StackTrace;
                GlobalContext.Properties[propertiesStacktrace] = logInfo.StackTrace;
                LogicalThreadContext.Properties[propertiesExecutionDuration] = logInfo.ExecutionDuration;
                LogicalThreadContext.Properties["Browser"] = $"{logInfo.BrowserInfo}-Custom value BrowserInfo";

                GlobalContext.Properties[propertiesOperatingAddress] = logInfo.OperatingAddress;
                GlobalContext.Properties[propertiesOS] = logInfo.OS;
                GlobalContext.Properties[propertiesTraceID] = logInfo.TraceID;
                GlobalContext.Properties[propertiesEventID] = logInfo.EventID;

                if (string.IsNullOrWhiteSpace(logInfo.ExceptionMessage))
                {
                    LogEncapsulation.Info(logInfo);
                }
                else
                {
                    LogEncapsulation.Error(logInfo);
                }

                return true;
            }
            catch (Exception e)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes(e.Message);
                using (FileStream fsWrite = new FileStream(path + "/errer.txt", FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                }
                return false;
            }
        }

        #endregion

    }


    #region LogEncapsulation
    public class LogEncapsulation
    {
        public static LogEncapsulation logEncapsulation = new LogEncapsulation();
        static LogEncapsulation()
        {
            string path = LogHelper.ConfigFilePath;
            log4net.Config.XmlConfigurator.Configure(new FileInfo(path));
        }
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void Debug(string message)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(message);
            }
        }
        public static void Debug(System.Exception ex)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(ex.Message.ToString() + ex.Source.ToString() + ex.TargetSite.ToString() + ex.StackTrace.ToString());
            }
        }
        public static void Error(Object message)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(message);
            }
        }
        public static void Fatal(Object message)
        {
            if (log.IsFatalEnabled)
            {
                log.Fatal(message);
            }
        }
        public static void Info(Object message)
        {
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }

        public static void Warn(Object message)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(message);
            }
        }
    }

    #endregion



}