using Joson.Elastic.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Joson.ElasticSearch.Api
{
    [ApiController]
    [Route("[controller]")]
    public class ElasticSearchController : ControllerBase
    {
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IActionResult Index(SearchInfo input, int pageIndex =1 )
        {
            pageIndex = pageIndex > 0 ? pageIndex : 1;

           
            var musts = JosonEsUtil.Must<SearchInfo>();
            if (!string.IsNullOrWhiteSpace(input.Name))
            {
                //musts.Add(c => c.Term(cc => cc.Field("School").Value(input.School)));
                musts.AddMatch("Name", input.Name);
            }

            if (!string.IsNullOrWhiteSpace(input.DicKeyword))
            {
                //musts.Add(c => c.MultiMatch(cc => cc.Fields(ccc => ccc.Fields(ced => new[] { ced.Title, ced.Name })).Query(input.DicKeyword)));
                musts.Add(c => c.MultiMatch(cc => cc.Query(input.DicKeyword).Fields(new[] { "title", "name" })));
            }

            var must2 = JosonEsUtil.Must<SearchInfo>();

 

            if (!string.IsNullOrWhiteSpace(input.Dic))
            {
                //musts.Add(c => c.Term(cc => cc.Field(ced => ced.Dic).Value(input.Dic)));
                must2.AddTerm("Dic", input.Dic);
            }

 

            if (input.PriceStart.HasValue)
            {
                //musts.Add(c => c.Range(cc => cc.Field(ccc => ccc.Price).GreaterThan((double)input.PriceStart.Value)));
                must2.AddGreaterThan("price", (double)input.PriceStart.Value);
            }

            if (input.PriceEnd.HasValue)
            {
                //musts.Add(c => c.Range(cc => cc.Field(ccc => ccc.Price).LessThanOrEquals((double)input.PriceEnd.Value)));
                must2.AddLessThanEqual("price", (double)input.PriceEnd.Value);
            }



            var client = JosonEsUtil.Client("http://127.0.0.1:9200", "SearchInfo");
            var result = client.Search<SearchInfo>(sd =>
                sd.Query(qcd => qcd
                        .Bool(cc => cc
                            .Must(musts)
                            .Filter(must2))
                        )
                        .From(pageIndex * (pageIndex -1 ))
                        .Take(10)
                    .Sort(sdd => sdd.Descending("price"))
                    .Sort(JosonEsUtil.Sort<SearchInfo>(c => c.Price))
            );

            var total = result.Total;
            var data = result.Documents;
            ViewBag.Total = total;
            return View(data);
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateIndex()
        {
            var result = JosonEsUtil.Client("http://127.0.0.1:9200").CreateIndex<SearchInfo>("SearchInfo");
            if (result)
            {
                return Content("创建成功");
            }
            else
            {
                return Content("创建失败，可能已存在同名索引");
            }
        }
    }
}
