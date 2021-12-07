using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace Net.Monitor.Helper
{

    #region WebApiTrackerAttribute

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebApiTrackerAttribute : ActionFilterAttribute
    {
        private readonly string Key = "_thisWebApiOnActionMonitorLog_";
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            WebApiMonitor MonLog = new WebApiMonitor();

            MonLog.ExecuteStartTime = DateTime.Now;
            //获取Action 参数
            MonLog.ActionParams = actionContext.ActionArguments;
            MonLog.HttpRequestHeaders = actionContext.Request.Headers.ToString();
            MonLog.HttpMethod = actionContext.Request.Method.Method;

            actionContext.Request.Properties[Key] = MonLog;

            var form = System.Web.HttpContext.Current.Request.Form;

            #region 如果参数是实体对象，获取序列化后的数据

            Stream stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
            Encoding encoding = Encoding.UTF8;
            stream.Position = 0;
            string responseData = "";
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                responseData = reader.ReadToEnd().ToString();
            }
            if (!string.IsNullOrWhiteSpace(responseData) && !MonLog.ActionParams.ContainsKey("__EntityParamsList__"))
            {
                MonLog.ActionParams["__EntityParamsList__"] = responseData;
            }

            #endregion
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            //WebApiMonitor MonLog = actionExecutedContext.Request.Properties[Key] as WebApiMonitor;
            //MonLog.ExecuteEndTime = DateTime.Now;
            //MonLog.ActionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            //MonLog.ControllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //LogHelper.Monitor(MonLog.GetLoginfo());

            //if (actionExecutedContext.Exception != null)
            //{
            //    string Msg = string.Format(@"
            //    请求【{0}Controller】的【{1}】产生异常：
            //    Action参数：{2}
            //    Http请求头:{3}
            //    客户端IP：{4},
            //    HttpMethod:{5},
            //    UserRequest:{6}",
            //    MonLog.ControllerName,
            //    MonLog.ActionName,
            //    MonLog.GetCollections(MonLog.ActionParams),
            //    MonLog.HttpRequestHeaders,
            //    MonLog.GetIP(),
            //    MonLog.HttpMethod,
            //    MonLog.UserRequest()

            //    );
            //    LogHelper.Error(Msg, actionExecutedContext.Exception);
            //}

        }
    }


    #endregion


}