
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
    //参考文档 https://blog.csdn.net/fairylym/article/details/106253194
    //https://blog.csdn.net/qq_35193189/article/details/82658394

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = true)]
    public class Log4NetFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        Stopwatch stop = null;

        public static Log4NetHelper logHelper = new Log4NetHelper();

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

                Log4NetHelper.logHelper.Write(LogEntitys);

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

                Log4NetHelper.logHelper.Write(LogEntitys);

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
 

}