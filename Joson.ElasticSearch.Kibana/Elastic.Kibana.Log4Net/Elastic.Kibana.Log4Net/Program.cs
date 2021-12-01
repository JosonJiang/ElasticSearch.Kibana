using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elastic.Kibana.Log4Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                 .ConfigureLogging((content,logging) =>
                 {
                     //�����ļ�
                     var path = System.IO.Directory.GetCurrentDirectory();
                     var log4netConfig = $"{path}\\log4net.config";

                     //logging.AddLog4Net(log4netConfig);

                     //ע��log4net
                     //���˵� System �� Microsoft ��ͷ�������ռ��µ���������ľ��漶�����µ���־
                     logging.AddFilter("System", LogLevel.Warning);
                     logging.AddFilter("Microsoft", LogLevel.Warning);
                     logging.AddLog4Net();

                 })
                //.UseLog4Net()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
 
    }

 

}
