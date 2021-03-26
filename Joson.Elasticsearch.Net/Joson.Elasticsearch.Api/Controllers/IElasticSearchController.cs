using Joson.Elastic.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Joson.ElasticSearch.Api
{
    [ApiController]
    [Route("[controller]")]
    public class IElasticSearchController : ControllerBase
    {

        private readonly ILogger<IElasticSearchController> _logger;

        String IndexName = "SearchInfo";
        List<Uri> _Uri = new List<Uri>();
        IClients _Clients;



        public IElasticSearchController()
        {

            _Uri.Add(new Uri("http://127.0.0.1:9200"));
            _Clients = new IClients(_Uri);
            //_Clients.nodes = _Uri;
        }


        //public IElasticSearchController(ILogger<IElasticSearchController> logger, IClients Clients ,List<Uri> Uri)
        //{
        //    _logger = logger;

        //    _Clients = Clients;
        //    _Uri = Uri;
        //    _Uri.Add(new Uri("http://127.0.0.1:9200"));

        //    _Clients.nodes = _Uri;
        //}


        [HttpPost]
        public IActionResult Index()
        {
            var r = _Clients.Index(new SearchInfo()
            {
                Id = 0,
                Name = "测试数据1",
                AddTime = DateTime.Now,
                CreateTime = DateTime.Now,
                Deleted = false,
                Dic = "测试内容测试内容测试内容测试内容测试内容测试内容测试内容测试内容",
                Dvalue = 222222222222.2222,
                Group = 1,
                PassingRate = float.MaxValue,
                State = 1
            }, IndexName);




            #region test data
            int index = 1;
            _Clients.Index(new SearchInfo()
            {
                Id = index++,
                Deleted = false,
                Name = "油菜花",
                Dic = "一种植物，叫油菜花。",
                AddTime = DateTime.Now
            }, IndexName);

            _Clients.Index(new SearchInfo()
            {
                Id = index++,
                Deleted = false,
                Name = "菜花",
                Dic = "一种蔬菜，叫菜花。",
                AddTime = DateTime.Now
            }, IndexName);

            _Clients.Index(new SearchInfo()
            {
                Id = index++,
                Deleted = false,
                Name = "黄瓜",
                Dic = "一种蔬菜，叫油黄瓜。",
                AddTime = DateTime.Now
            }, IndexName);

            _Clients.Index(new SearchInfo()
            {
                Id = index++,
                Deleted = false,
                Name = "苹果",
                Dic = "一种水果，叫苹果。",
                AddTime = DateTime.Now
            }, IndexName);

            _Clients.Index(new SearchInfo()
            {
                Id = index++,
                Deleted = false,
                Name = "橘子",
                Dic = "一种水果，叫桔子。",
                AddTime = DateTime.Now
            }, IndexName);

            #endregion

            return Ok(r);

        }

        [HttpPost("IndexMany", Name = "IndexMany")]
        public IActionResult Indexs()
        {

            var lists = new List<SearchInfo>();
            for (int i = 0; i < 500; i++)
            {
                lists.Add(new SearchInfo()
                {
                    Id = i,
                    Name = "测试数据" + i,
                    AddTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Deleted = false,
                    Dic = i + "测试内容测试内容测试内容测试内容测试内容测试内容测试内容测试内容",
                    Dvalue = 222222222222.2222,
                    Group = 1,
                    PassingRate = float.MaxValue,
                    State = 1
                });
            }
            _Clients.IndexMany(lists, IndexName);

            return Ok(lists);

        }

        [HttpDelete]
        public void DeleteIndex()
        {
            _Clients.DeleteIndex(IndexName);
        }


        [HttpDelete("DeleteByQuery/{oValue}")]
        public void DeleteByQuery(String oValue)
        {
            Func<TermQueryDescriptor<SearchInfo>, ITermQuery> selector = X => X.Field(Q => Q.Name).Value(oValue);
            _Clients.DeleteByQuery(IndexName, selector);
        }


        [HttpGet("UpdateByWhere/{IndexName}/{oValue}")]
        public IActionResult Get(string oValue,string nValue)
        {
            string script = "ctx._source.Name = params.newName;";
            var scriptParams = new Dictionary<string, object> { { "newName", nValue } };

            Func<TermQueryDescriptor<SearchInfo>, ITermQuery> selector = X => X.Field(Q => Q.Name).Value(oValue);
     
            var r = _Clients.UpdateByWhere<SearchInfo>(IndexName, selector, script, scriptParams);
 
            return Ok(r);

        }

        [HttpGet("UpdateByMap/{IndexName}/{nKey}")]
        public IActionResult GetUpdateMap(string nKey, string nValue)
        {
            var r = _Clients.UpdateMap<SearchInfo>(IndexName, nKey, nValue);


            var lists = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                lists.Add(new
                {
                    Id = i,
                    Name = $"{nKey}" + i,
                    AddTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Deleted = false,
                    Dic = i + $"{nValue}",
                    nKey = $"{nKey}" + i,
                    nValue = $"{nValue}" + i
                });
            }
            _Clients.IndexMany(lists, IndexName);

            return Ok(r);
        }



        [HttpGet("{IndexName}")]
        public IActionResult Get(String IndexName, int ID) => ID == 0 ? GetByName(IndexName) : GetByID(IndexName, ID);

        private IActionResult GetByName(String IndexName)
        {
            var Version = _Clients.GetVersion<SearchInfo>(IndexName, 1);
            var r = _Clients.Get<SearchInfo>(IndexName, 1);

            return Ok(r);

        }

        private IActionResult GetByID(String IndexName, int ID)
        {
            var Version = _Clients.GetVersion<SearchInfo>(IndexName, ID);
            var r = _Clients.Get<SearchInfo>(IndexName, 1);

            return Ok(r);

        }


        [HttpGet("MatchQuery/{IndexName}")]
        public IActionResult Search(String IndexName)
        {
            var _client = _Clients.CreateElasticClient(_Uri);
            //立马刷新数据 否则有默认1s延迟，无法查询出来
            _client.Indices.Refresh(IndexName);

            var countResult = _client.Count<SearchInfo>(s => s.Index(IndexName));


            var result = _client.Search<SearchInfo>(s => s
                .Index(IndexName)
                       .Query(q => q
                           .Bool(b => b
                               .Must(m => m
                               .Match(mt => mt
                                   .Field(fd => fd.Dic)
                                   .Query("油菜花")
                                    )
                                )
                           )
                       )
               .DocValueFields(fdf => fdf.Fields(fd => fd.Dic, fd => fd.Name))//查看实际内容 结果在 result.Fields 中
               .Size(10)
       );

            var sCount = result.Hits.Count;

            var results = _client.Search<SearchInfo>(s => s
                .Index(IndexName)
                       .Query(q => q
                           .Bool(b => b
                               .Must(m => m
                               .Match(mt => mt
                                   .Field(fd => fd.Dic)
                                   .Query("苹果")
                                    )
                                )
                           )
                       )
               .DocValueFields(fdf => fdf.Fields(fd => fd.Dic, fd => fd.Name))//查看实际内容 结果在 result.Fields 中
               .Size(10)
       );

            var iCount = results.Hits.Count;

            return Ok();

        }


        /// <summary>
        /// Wildcard 通配符匹配
        /// </summary>
        /// <param name="IndexName"></param>
        /// <returns></returns>
        [HttpGet("Wildcard/{IndexName}")]
        public IActionResult Wildcard(String IndexName)
        {
            var _client = _Clients.CreateElasticClient(_Uri);
            //立马刷新数据 否则有默认1s延迟，无法查询出来
            _client.Indices.Refresh(IndexName);

            var countResult = _client.Count<SearchInfo>(s => s.Index(IndexName));


            var result = _client.Search<SearchInfo>(s => s
                .Index(IndexName)
                       .Query(q => q
                           .Bool(b => b
                                   .Must(m => m
                                       .Wildcard(qs => qs.Field(fd => fd.DicKeyword).Value("*油菜花*"))//Wildcard 要用keyword类型 
                                    )
                           )
                       )
               .DocValueFields(fdf => fdf.Fields(fd => fd.Dic, fd => fd.Name))
               .Size(10)
       );

            var sCount = result.Hits.Count;

            var results = _client.Search<SearchInfo>(s => s
                .Index(IndexName)
                       .Query(q => q
                           .Bool(b => b
                               .Must(m => m
                                    .Wildcard(qs => qs.Field(fd => fd.DicKeyword).Value("*蔬菜*"))//Wildcard 要用keyword类型
                                )
                           )
                       )
               .DocValueFields(fdf => fdf.Fields(fd => fd.Dic, fd => fd.Name))
               .Size(10)
               );

            var iCount = results.Hits.Count;

            return Ok();

        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        [HttpGet("CreateIndex/{IndexName}", Name = "CreateIndex")]
        public IActionResult CreateIndex()
        {
            var result = _Clients.CreateIndex<SearchInfo>(IndexName);
            if (result.IsValid)
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
