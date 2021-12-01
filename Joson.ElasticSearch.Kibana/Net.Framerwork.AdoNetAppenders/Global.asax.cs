using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Net.Framerwork.AdoNetAppenders
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //应用程序启动时，自动加载配置log4Net  
            XmlConfigurator.Configure();
            //var FileLog4net = new System.IO.FileInfo(Server.MapPath("~/Properties/") + @"log4net.config");
            //XmlConfigurator.ConfigureAndWatch(FileLog4net);



        }
    }
}
