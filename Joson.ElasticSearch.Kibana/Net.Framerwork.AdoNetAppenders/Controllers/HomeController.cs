using log4net;
using log4net.Appender;
using log4net.ElasticSearch;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Net.Framerwork.AdoNetAppenders.Controllers
{
    public class HomeController : Controller
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["SQLite"].ConnectionString;
        public ActionResult Index()
        {




            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Net.Framerwork.AdoNetAppenders.Properties.log4net.config"))
            {
                // stream is NOT null
                log4net.Config.XmlConfigurator.Configure(stream);
            }

            Hierarchy hier = LogManager.GetRepository() as Hierarchy;



            if (hier != null)
            {
                //get ADONetAppender
                var adoAppender = (AdoNetAppender)hier.GetAppenders()
                    .Where(appender => appender.Name.Equals("SQLServerAppender", StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                adoAppender = (AdoNetAppender)hier.GetAppenders()
                    .Where(appender => appender.Name.Equals("SQLiteNetAppender", StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();


                ElasticSearchAppender ElasticSearchAppenders = (ElasticSearchAppender)hier.GetAppenders()
                    .Where(appender => appender.Name.Equals("ElasticSearchAppender", StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (adoAppender == null)
                {

                    //connStr = adoAppender.ConnectionString;
                    //SQLiteConnection cn = new SQLiteConnection(connStr);

                    ////按照路径创建数据库文件
                    ////cn.Open();
                    ////创建数据库表
                    //if (cn.State != System.Data.ConnectionState.Open)
                    //{
                    //    cn.Open();//打开数据库
                    //    SQLiteCommand cmd = new SQLiteCommand();
                    //    cmd.Connection = cn;//把 SQLiteCommand的 Connection和SQLiteConnection 联系起来
                    //    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Joson(ID varchar(4),score int)";//输入SQL语句
                    //    cmd.ExecuteNonQuery();//调用此方法运行

                    //    cmd.CommandText = "INSERT INTO Log(Date, Level, Logger, Message) VALUES('@Date', '@Level', '@Logger', '@Message')";
                    //    cmd.ExecuteNonQuery();//调用此方法运行
                    //}
                    //cn.Close();


                    // update connectionstring
                    //adoAppender.ConnectionString = configuration.GetConnectionString(ConnectionStringNames.Log).ConnectionString;
                    adoAppender.ConnectionString = "设置数据库连接";
                    //refresh settings of appender
                    adoAppender.ActivateOptions();

                }

            }



            ILog log = log4net.LogManager.GetLogger("Log");
            log.Error("ActionResult -Index");




            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }



    public static class LogHelp
    {
        public static void WriteError(Exception ex)
        {
            ILog log = log4net.LogManager.GetLogger("Log");
            log.Error(ex.Message, ex);
        }
    }

}