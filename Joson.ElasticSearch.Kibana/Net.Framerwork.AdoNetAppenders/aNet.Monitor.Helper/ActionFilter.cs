
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Web;

//using WebFilter = System.Web.Http.Filters;


using System.Web.Mvc;


namespace Net.Monitor.Helper
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = true)]
    public class Log4NetFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        Stopwatch stop = null;

        public static LogHelper logHelper = new LogHelper();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            stop = new Stopwatch();
            stop.Start();
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            try
            {
                if (stop.IsRunning)
                {
                    stop.Stop();
                }

                long milliseconds = stop.ElapsedMilliseconds;
                //登录ID
                var MemberID = filterContext.HttpContext.Session["MemberID"];
                var UserNick = filterContext.HttpContext.Session["UserNick"];
                //请求参数post
                string RequestForm = filterContext.HttpContext.ApplicationInstance.Request.Form.ToString();
                //请求参数 get
                var QueryString = filterContext.HttpContext.ApplicationInstance.Request.QueryString.ToString();
                //客户端信息
                var Browser = filterContext.HttpContext.Request.Browser;

                int? nullId = null;

                var LogEntitys = new LogEntity()
                {
                    TraceID = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).AnonymousID ?? string.Empty,
                    EventID = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).AnonymousID ?? string.Empty,

                    Category = Convert.ToString(HttpContextCategory.Response),

                    MemberID = MemberID == null ? nullId : Convert.ToInt32(MemberID),
                    UserNick = Convert.ToString(UserNick) ?? HttpContextClient.Host,
                    ExecutionDuration = milliseconds,
                    ClientIP = HttpContextClient.IP,
                    ClientName = HttpContextClient.Host,
                    OperatingAddress = HttpContextClient.GetClientALLIP(),
                    Action = $"{filterContext.RequestContext.RouteData.Values["controller"].ToString()}|{ filterContext.RequestContext.RouteData.Values["action"].ToString()}",
                    Method = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).RequestType,
                    ServiceName = filterContext.Controller.GetType().ToString(),
                    OS = $"{Browser.Type}/{Browser.Version}/{Browser.Platform}丨{Browser.EcmaScriptVersion.Major.ToString()}",
                    BrowserInfo = $"{((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).UserAgent}",
                    ExceptionMessage = filterContext.Exception?.InnerException?.Message ?? string.Empty,
                    Exception = filterContext.Exception?.Message ?? string.Empty,
                    StackTrace = filterContext.Exception?.StackTrace ?? string.Empty,
                    CustomData = HttpContextClient.UserRequest,
                    Parameters = QueryString

                };

                LogHelper.logHelper.Write(LogEntitys);

            }
            catch (Exception ex)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                byte[] myByte = Encoding.UTF8.GetBytes(ex.Message + ex.StackTrace);
                using (FileStream fsWrite = new FileStream(path + "/Response-Errer.txt", FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                }
            }
            finally
            {
                base.OnResultExecuted(filterContext);
            }


        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!stop.IsRunning) { stop.Start(); }

            base.OnResultExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                stop.Stop();
                long milliseconds = stop.ElapsedMilliseconds;

                //登录ID
                var MemberID = filterContext.HttpContext.Session["MemberID"];
                var UserNick = filterContext.HttpContext.Session["UserNick"];
                //请求参数post
                string RequestForm = filterContext.HttpContext.ApplicationInstance.Request.Form.ToString();
                //请求参数 get
                var QueryString = filterContext.HttpContext.ApplicationInstance.Request.QueryString.ToString();
                //客户端信息
                var Browser = filterContext.HttpContext.Request.Browser;

                int? nullId = null;

                var LogEntitys = new LogEntity()
                {
                    TraceID = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).AnonymousID ?? string.Empty,
                    EventID = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).AnonymousID ?? string.Empty,

                    Category = Convert.ToString(HttpContextCategory.Request),

                    MemberID = MemberID == null ? nullId : Convert.ToInt32(MemberID),
                    UserNick = Convert.ToString(UserNick) ?? HttpContextClient.Host,
                    ExecutionDuration = milliseconds,
                    ClientIP = HttpContextClient.IP,
                    ClientName = HttpContextClient.Host,
                    OperatingAddress = HttpContextClient.GetClientALLIP(),
                    Action = $"{filterContext.RequestContext.RouteData.Values["controller"].ToString()}|{ filterContext.RequestContext.RouteData.Values["action"].ToString()}",
                    Method = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).RequestType,
                    ServiceName = filterContext.Controller.GetType().ToString(),
                    OS = $"{Browser.Type}/{Browser.Version}/{Browser.Platform}丨{Browser.EcmaScriptVersion.Major.ToString()}",
                    BrowserInfo = $"{((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).UserAgent}",
                    ExceptionMessage = filterContext.Exception?.InnerException?.Message ?? string.Empty,
                    Exception = filterContext.Exception?.Message ?? string.Empty,
                    StackTrace = filterContext.Exception?.StackTrace ?? string.Empty,
                    CustomData = HttpContextClient.UserRequest,
                    Parameters = QueryString

                };

                LogHelper.logHelper.Write(LogEntitys);

            }
            catch (Exception ex)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                byte[] myByte = Encoding.UTF8.GetBytes(ex.Message + ex.StackTrace);
                using (FileStream fsWrite = new FileStream(path + "/Request-Errer..txt", FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                }
            }
            finally
            {
                base.OnActionExecuted(filterContext);
            }
        }
    }





    #region WebApiTrackerAttribute

    ////[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    //public class WebApiTrackerAttribute : WebFilter.ActionFilterAttribute
    //{
    //    private readonly string Key = "_thisWebApiOnActionMonitorLog_";
    //    public override void OnActionExecuting(ActionExecutingContext actionContext)
    //    {
    //        //base.OnActionExecuting(actionContext);

    //        WebApiMonitor MonLog = new WebApiMonitor();

    //        MonLog.ExecuteStartTime = DateTime.Now;
    //        //获取Action 参数
    //        MonLog.ActionParams = actionContext.ActionArguments;
    //        MonLog.HttpRequestHeaders = actionContext.Request.Headers.ToString();
    //        MonLog.HttpMethod = actionContext.Request.Method.Method;

    //        actionContext.Request.Properties[Key] = MonLog;

    //        var form = System.Web.HttpContext.Current.Request.Form;

    //        #region 如果参数是实体对象，获取序列化后的数据

    //        Stream stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
    //        Encoding encoding = Encoding.UTF8;
    //        stream.Position = 0;
    //        string responseData = "";
    //        using (StreamReader reader = new StreamReader(stream, encoding))
    //        {
    //            responseData = reader.ReadToEnd().ToString();
    //        }
    //        if (!string.IsNullOrWhiteSpace(responseData) && !MonLog.ActionParams.ContainsKey("__EntityParamsList__"))
    //        {
    //            MonLog.ActionParams["__EntityParamsList__"] = responseData;
    //        }

    //        #endregion
    //    }

    //    public override void OnActionExecuted(WebFilter.HttpActionExecutedContext actionExecutedContext)
    //    {
    //        WebApiMonitor MonLog = actionExecutedContext.Request.Properties[Key] as WebApiMonitor;
    //        MonLog.ExecuteEndTime = DateTime.Now;
    //        MonLog.ActionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
    //        MonLog.ControllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
    //        LogHelper.Monitor(MonLog.GetLoginfo());

    //        if (actionExecutedContext.Exception != null)
    //        {
    //            string Msg = string.Format(@"
    //            请求【{0}Controller】的【{1}】产生异常：
    //            Action参数：{2}
    //            Http请求头:{3}
    //            客户端IP：{4},
    //            HttpMethod:{5},
    //            UserRequest:{6}",
    //            MonLog.ControllerName,
    //            MonLog.ActionName,
    //            MonLog.GetCollections(MonLog.ActionParams),
    //            MonLog.HttpRequestHeaders,
    //            MonLog.GetIP(),
    //            MonLog.HttpMethod,
    //            MonLog.UserRequest()

    //            );
    //            LogHelper.Error(Msg, actionExecutedContext.Exception);
    //        }

    //    }
    //}


    #endregion

}