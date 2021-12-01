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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLogs = NLog;
using NLog.Extensions.Logging;

namespace Elastic.Kibana.NLog
{


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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //https://www.cnblogs.com/jimmyLei/p/11284598.html
            //log.AddNLog();
            //env.ConfigureNLog("NLog.config");//配置NLog文件

            #region 将日志记录到数据库

            var NLOGDataBase = Configuration.GetConnectionString("DefaultConnection");

            //var configFileName = $"nlog.Joson{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.config";
            //NLogs.LogManager.LoadConfiguration(configFileName).GetCurrentClassLogger();


            NLogs.LogManager.Configuration.Variables["connectionString"] = NLOGDataBase;
            NLogs.Targets.DatabaseTarget databaseTarget = NLogs.LogManager.Configuration.FindTargetByName<NLogs.Targets.DatabaseTarget>("Database");
            databaseTarget.ConnectionString = Configuration.GetConnectionString("DefaultConnection");


            NLogs.LogManager.Configuration.Variables["NLOG_CONNECTION_STRING"] = NLOGDataBase;
            NLogs.LogManager.Configuration.FindTargetByName<NLogs.Targets.DatabaseTarget>("SQLServerCreateNLog").ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //避免日志中的中文输出乱码 

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
