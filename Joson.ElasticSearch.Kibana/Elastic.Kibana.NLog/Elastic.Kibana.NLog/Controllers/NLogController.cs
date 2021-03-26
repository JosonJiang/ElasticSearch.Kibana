using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elastic.Kibana.NLog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NLogController : ControllerBase
    {

        //message:"Get executed at"
        //message:"WeatherForecastController"

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<NLogController> _logger;

        public NLogController(ILogger<NLogController> logger)
        {
            _logger = logger;
        }


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


        [HttpGet("Get/{ID}")]
        public async Task<string> Gets(int ID) => $"Gets {ID}";



        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Get  WeatherForecastController executed at {date}", DateTime.UtcNow);

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
