using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace Elastic.Kibana.Log4Net
{

    using log4net.Appender;
    using log4net.ElasticSearch;
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            #region 添加log4net

            try
            {
                throw new Exception("使用log4net.ElasticSearch，不使用 log4stash 操作 ElasticSearch");

                //使用 log4stash 
                var Log4NetConfig = $"log4net.Joson{(env.IsDevelopment() ? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") : String.Empty)}.config";
                loggerFactory.AddLog4Net(new Log4NetProviderOptions { Name = "Log4Net", Log4NetConfigFileName = Log4NetConfig });



            }
            catch (Exception)
            {
                #region log4net.ElasticSearch 设置数据库

                log4net.Repository.Hierarchy.Hierarchy Hierarchy = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;

                if (Hierarchy != null)
                {
                    //get ADONetAppender
                    var SQLServerAppender = (AdoNetAppender)Hierarchy.GetAppenders()
                        .Where(appender => appender.Name.Equals("SQLServerAppender", StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                    var SQLiteNetAppender = (AdoNetAppender)Hierarchy.GetAppenders()
                         .Where(appender => appender.Name.Equals("SQLiteNetAppender", StringComparison.InvariantCultureIgnoreCase))
                         .FirstOrDefault();


                    ElasticSearchAppender ElasticSearchAppender = (ElasticSearchAppender)Hierarchy.GetAppenders()
                        .Where(appender => appender.Name.Equals("ElasticSearchAppender", StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                    ElasticSearchAppender ElasticSearchCustomaryAppender = (ElasticSearchAppender)Hierarchy.GetAppenders()
                        .Where(appender => appender.Name.Equals("ElasticSearchCustomaryAppender", StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();


                    if (SQLServerAppender != null)
                    {
                        //SQLServerAppender.ConnectionString = "数据库连接";

                        string SQLServer = Configuration.GetConnectionString("SQLServer");
                        //更新数据库连接
                        SQLServerAppender.ConnectionString = Configuration.GetConnectionString(SQLServer);
                        //SQLServerAppender.ConnectionStringName = "SQLServer";
                        //刷新AdoNetAppender
                        SQLServerAppender.ActivateOptions();

                    }


                    if (SQLiteNetAppender != null)
                    {
                        string SQLite = Configuration.GetConnectionString("SQLite");
                        SQLiteNetAppender.ConnectionString = Configuration.GetConnectionString(SQLite);
                        SQLiteNetAppender.ActivateOptions();

                    }

                    if (ElasticSearchAppender != null)
                    {
                        string ElasticSearch = Configuration.GetConnectionString("ElasticSearch");
                        ElasticSearchAppender.ConnectionString = Configuration.GetConnectionString(ElasticSearch);
                        ElasticSearchAppender.ActivateOptions();

                    }

                    if (ElasticSearchCustomaryAppender != null)
                    {
                        string ElasticSearch = Configuration.GetConnectionString("ElasticSearchCustomary");
                        ElasticSearchCustomaryAppender.ConnectionString = Configuration.GetConnectionString(ElasticSearch);
                        ElasticSearchCustomaryAppender.ActivateOptions();

                    }
                }

                #region 动态修改log4net设置

                //https://www.cnblogs.com/jinzhao/archive/2009/08/06/1540214.html
                //var log4netConfig = new FileInfo("~/log4net.Relase.config");
                //log4net.Config.XmlConfigurator.ConfigureAndWatch(log4netConfig); 

                #endregion

                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Elastic.Kibana.Log4Net.log4net.Relase.config"))
                {
                    log4net.Config.XmlConfigurator.Configure(stream);
                }

                #endregion
            }

            #endregion


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
