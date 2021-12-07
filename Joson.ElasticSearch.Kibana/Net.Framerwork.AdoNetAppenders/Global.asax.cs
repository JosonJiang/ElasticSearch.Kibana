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

            //Ӧ�ó�������ʱ���Զ���������log4Net  
            //XmlConfigurator.Configure();
            //var FileLog4net = new System.IO.FileInfo(Server.MapPath("~/") + @"log4net.config");
            //XmlConfigurator.Configure(FileLog4net);

            var ConfigFilePath = Net.Monitor.Helper.Log4NetHelper.ConfigFilePath ?? Server.MapPath("~/Properties/") + @"log4net.config";
            var FileLog4net = new System.IO.FileInfo(ConfigFilePath);
            XmlConfigurator.ConfigureAndWatch(FileLog4net);


        }
    }
}
