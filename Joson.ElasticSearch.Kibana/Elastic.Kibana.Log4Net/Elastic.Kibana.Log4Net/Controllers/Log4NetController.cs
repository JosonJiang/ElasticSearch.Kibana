using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elastic.Kibana.Log4Net.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Log4NetController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Log4NetController> _logger;

        public Log4NetController(ILogger<Log4NetController> logger)
        {
            _logger = logger;
        }


        [HttpGet("Get/{ID}")]
        public async Task<string> Gets(int ID) => $"Gets {ID}";


        [HttpGet("{ID}")]
        public Task<string> Get(string ID)
        {
            //Task.Run(() => Get());

            _logger.LogDebug("LogDebug {ID} executed at {date}", ID, DateTime.UtcNow);
            _logger.LogError("LogError {ID} executed at {date}", ID, DateTime.UtcNow);
            _logger.LogTrace("LogTrace {ID} executed at {date}", ID, DateTime.UtcNow);
            _logger.LogWarning("LogWarning {ID} executed at {date}", ID, DateTime.UtcNow);
            _logger.LogInformation("Get {ID} executed at {date}", ID, DateTime.UtcNow);

            _logger.LogCritical("LogCritical {ID} executed at {date}", ID, DateTime.UtcNow);

            Task<string> T = Task.Run(() => $" Get {ID} executed at {DateTime.UtcNow}");
            return T;
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Console.WriteLine("测试日志");

            log4net.LogManager.CreateRepository("MesIRepository");
            log4net.ILog logs = Log4Net.Log4NetExtensions.GetLog();
            logs.Debug("动态给log4net添加日志类型");
            logs.Info("动态生成日志配置项  生成的日志配应该是保存在内存中的，如果停止运行会消失，不会保存到log4net.config文件中");


            _logger.LogWarning("测试日志");
            _logger.LogInformation("JosonRequest /WeatherForecast");
            _logger.LogError(new Exception("出错啦！！！"), "request api/values");
            _logger.LogError("这是一个简单日志测试");


            _logger.LogDebug("调试信息 executed at {date}", DateTime.UtcNow);
            _logger.LogError("错误信息 executed at {date}", DateTime.UtcNow);
            _logger.LogTrace("追踪; 追溯; 查出; 找到; 发现; 追究; 描绘(事物的过程或发展); 追述; 记述 executed at {date}", DateTime.UtcNow);
            _logger.LogWarning("警告; 告诫; 提醒注意(可能发生的事); 使警惕; 劝告(使有所防备) executed at {date}", DateTime.UtcNow);
            _logger.LogCritical("关键的; 批判性的; 批评的; 挑剔的; 极重要的; 至关紧要的; 严重的; 不稳定的; 可能有危险的; executed at {date}", DateTime.UtcNow);

 

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
