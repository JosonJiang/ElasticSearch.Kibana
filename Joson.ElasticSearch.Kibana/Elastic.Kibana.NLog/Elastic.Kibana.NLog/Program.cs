using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elastic.Kibana.NLog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureLogging((hostingContext, loggingBuilder) =>
                 {
                     var configFileName = $"nlog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.config";

                     loggingBuilder.AddJsonConsole(X =>{ X.IncludeScopes = true; }).AddDebug().AddNLog(configFileName);
                     loggingBuilder.AddConsole(x => x.IncludeScopes = true).AddDebug();

                 })
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
