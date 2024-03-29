﻿using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Elastic.Kibana.Serilog.Models;

namespace Elastic.Kibana.Serilog.Controllers
{




    public class HomeController : Controller
    {
        #region 在 Kibana 中搜索的容易程度以及 ElasticSearch 的强大功能

        //level:"error"
        //level:"Information"
        //message:"oh there"
        //fields.ActionName:"Elastic.Kibana.Serilog.Controllers.HomeController.Index" 

        #endregion


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("HomeController Index executed at {date}", DateTime.UtcNow);

            try
            {           
                throw new Exception("Some bad code was executed");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"oh there! :{ex.Message}| {DateTime.UtcNow}");
                _logger.LogError(ex, "An unknown error occurred on the Index action of the HomeController");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
