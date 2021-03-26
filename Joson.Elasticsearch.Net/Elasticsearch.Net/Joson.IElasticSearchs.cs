
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
namespace Joson.Elastic.Search
{

    public class IClients
    {
        private readonly IElasticClient elasticClient;
        public List<Uri> nodes { get; set; }

        public IClients(List<Uri> nodes)
        {
            elasticClient = CreateElasticClient(nodes);
        }
        public IConnectionPool CreateConnectionPool(List<Uri> nodes)
        {

            //支持ping 说明能够发现节点的状态
            //支持嗅探 说明能够发现新的节点

            //应用于已知集群，请求时随机请求各个正常节点，支持ping 不支持嗅探
            IConnectionPool pool = new StaticConnectionPool(nodes);

            //IConnectionPool pool=new SingleNodeConnectionPool(nodes[0]);

            //可动态嗅探集群 ，随机请求 支持嗅探、ping
            //IConnectionPool pool = new SniffingConnectionPool(nodes);

            //选择一个可用节点作为请求主节点，支持ping 不支持嗅探
            //IConnectionPool pool = new StickyConnectionPool(nodes);

            //选择一个可用节点作为请求主节点，支持ping 支持嗅探
            //IConnectionPool pool=new StickySniffingConnectionPool(nodes);
            return pool;
        }
        public IElasticClient CreateElasticClient(List<Uri> nodes)
        {
            var pool = CreateConnectionPool(nodes);

            var settings = new ConnectionSettings(pool);
            //验证 未开启
            //settings.BasicAuthentication("username", "password");
            //验证证书
            //settings.ClientCertificate("");
            //settings.ClientCertificates(new X509CertificateCollection());
            //settings.ServerCertificateValidationCallback();

            //开启 第一次使用时进行嗅探，需链接池支持
            //settings.SniffOnStartup(false);

            //链接最大并发数
            //settings.ConnectionLimit(80);
            //标记为死亡节点的超时时间
            //settings.DeadTimeout(new TimeSpan(10000));
            //settings.MaxDeadTimeout(new TimeSpan(10000));
            //最大重试次数
            //settings.MaximumRetries(5);
            //重试超时时间 默认是RequestTimeout
            //settings.MaxRetryTimeout(new TimeSpan(50000));
            //禁用代理自动检测
            //settings.DisableAutomaticProxyDetection(true);

            //禁用ping 第一次使用节点或使用被标记死亡的节点进行ping
            settings.DisablePing(false);
            //ping 超时设置
            //settings.PingTimeout(new TimeSpan(10000));
            //选择节点
            //settings.NodePredicate(node =>
            //{
            //    
            //        return true;
            //    
            //});
            //默认操作索引
            //settings.DefaultIndex("");
            //字段名规则 与model字段同名
            //settings.DefaultFieldNameInferrer(name => name);
            //根据Type 获取类型名
            //settings.DefaultTypeNameInferrer(name => name.Name);
            //请求超时设置
            //settings.RequestTimeout(new TimeSpan(10000));
            //调试信息
            //settings.DisableDirectStreaming(true);
            //调试信息
            //settings.EnableDebugMode((apiCallDetails) =>
            //{
            //    //请求完成 返回 apiCallDetails

            //});
            //抛出异常
            settings.ThrowExceptions(true);
            //settings.OnRequestCompleted(apiCallDetails =>
            //{
            //    //请求完成 返回 apiCallDetails
            //});
            //settings.OnRequestDataCreated(requestData =>
            //{
            //    //请求的数据创建完成 返回请求的数据

            //});
            return new ElasticClient(settings);
        }


        public CreateIndexResponse CreateIndex<T>(string indexName) where T : class
        {
            try
            {
                IIndexState indexState = new IndexState
                {
                    Settings = new IndexSettings
                    {
                        NumberOfReplicas = 1,       //副本数
                        NumberOfShards = 2          //分片数
                    }
                };

                var response = elasticClient.Indices.Create(indexName.ToLower(), index => index.InitializeUsing(indexState).Map<T>(x => x.AutoMap()));

                return response;
            }
            catch (Exception e)
            {
                throw e;   
            }


        }

        public DeleteIndexResponse DeleteIndex(string indexName)
        {
            var response = elasticClient.Indices.Delete(indexName.ToLower());
            return response;
        }

        public ExistsResponse IndexExists(string indexName)
        {
            var response = elasticClient.Indices.Exists(indexName.ToLower());
            return response;
        }

        public PutMappingResponse Map<T>(string indexName) where T : class
        {
            //根据对象类型自动映射
            var response = elasticClient.Map<T>(m => m.Index(indexName.ToLower()).AutoMap());
            return response;
        }


        public PutMappingResponse UpdateMap<T>(string indexName, String NewField = "NewField", String NewFieldText = "NewFieldText") where T : class
        {
            //新增字段
            var response = elasticClient.Map<T>(m => m
            .Index(indexName.ToLower())
            .Properties(p => p
                            .Keyword(s => s.Name(NewField).Index(true))
                            .Text(s => s.Name(NewFieldText).Index(false))
                    )
            );

            return response;
        }


        public T Get<T>(string indexName, int id) where T : class
        {
            DocumentPath<T> path = new DocumentPath<T>(id);
            var response = elasticClient.Get<T>(path, s => s.Index(indexName.ToLower()));
            return response.Source;
        }


        public IndexResponse Index<T>(T data, string indexName) where T : class
        {
            IndexResponse response = null;
            try
            {
                //写入数据，指定索引
                response = elasticClient.Index(data, s => s.Index(indexName.ToLower()));

            }
            catch (Exception ex)
            {
                var e = (ElasticsearchClientException)ex;
                string msg = e.Response.OriginalException.Message;

            }
            return response;

        }
        public BulkResponse IndexMany<T>(List<T> datas, string indexName) where T : class
        {
            //写入数据，指定索引
            BulkResponse bulkResponse = elasticClient.IndexMany(datas, indexName.ToLower());

            return bulkResponse;


        }


        public DeleteResponse Delete<T>(string indexName, int id) where T : class
        {
            try
            {
                DocumentPath<T> deletePath = new DocumentPath<T>(id);
                var response = elasticClient.Delete(deletePath, s => s.Index(indexName.ToLower()));
                return response;

            }
            catch (ElasticsearchClientException ex)
            {
                if (ex.Response.HttpStatusCode == 404)
                {
                    //404
                }
                throw;
            }

        }
        public DeleteByQueryResponse DeleteByQuery<T>(string indexName, Func<TermQueryDescriptor<T>, ITermQuery> selector) where T : class
        {
            DeleteByQueryResponse deleteByQueryResponse = elasticClient.DeleteByQuery<T>(s => s.Index(indexName.ToLower()).Query(q => q.Term(selector)));
            return deleteByQueryResponse;
        }


        public UpdateResponse<T> Update<T>(string indexName, int id, T model) where T : class
        {
            //完整更新
            DocumentPath<T> path = new DocumentPath<T>(id);
            var response = elasticClient.Update(path, (p) => p.Index(indexName.ToLower()).Doc(model));
            return response;

        }




        public UpdateByQueryResponse UpdateByWhere<T>(string indexName, Func<TermQueryDescriptor<T>, ITermQuery> selector, string script, Dictionary<string, object> scriptParams) where T : class
        {
            //elasticsearch.yml 设置 script.inline: true 

            //var script="ctx._source.Name = params.newName;"
            //var scriptParams = new Dictionary<string, object> { { "newName", "新的名字" } };

            UpdateByQueryResponse response = elasticClient.UpdateByQuery<T>(
                s =>
                    s.Index(indexName.ToLower())
                        .SearchType(SearchType.QueryThenFetch)
                        .Query(Q => Q.Term(selector))
                        .Script(X => X.Source(script)
                                      .Params(scriptParams))
                        );

            return response;
        }



        public long GetVersion<T>(string indexName, int id) where T : class
        {
            DocumentPath<T> path = new DocumentPath<T>(id);
            var response = elasticClient.Get<T>(path, s => s.Index(indexName.ToLower()));
            return response.Version;
        }






        public void VersionLock<T>(string indexName, T Model, Func<TermQueryDescriptor<T>, ITermQuery> selector) where T : class
        {
            //查询到版本号
            var result = elasticClient.Search<T>(
                s =>
                    s.Index(indexName.ToLower())
                        .Query(q => q.Term(selector)).Size(1)
                        .Version()//结果中包含版本号
                        );

            foreach (var s in result.Hits)
            {
                Console.WriteLine(s.Id + "  -  " + s.Version);
            }

            var path = new DocumentPath<T>(1);
            //更新时带上版本号 如果服务端版本号与传入的版本好相同才能更新成功
            var response = elasticClient.Update(path, (p) => p
                .Index(indexName.ToLower())
                .IfPrimaryTerm(2)//限制es中版本号为2时才能成功
                .Doc(Model)
                );
        }


        public void SearchScanScroll<T>(string indexName) where T : class
        {
            //Action<int> sc1 = (id) =>
            //{
            //    string scrollid = "";
            //    //todo:5.x 多了Slice设置 移除SearchType.Scan
            //    var result = elasticClient.Search<T>(s => s.Index(indexName).Query(q => q.MatchAll())
            //        .Size(15)
            //        .Sort(st => st.Descending(ds => ds.Id))
            //        .Scroll("1m")
            //        //id从0开始 0,1,2...
            //        //length=max
            //        //例：max=3 id=0,id=1,id=2
            //        .Slice(sl => sl.Id(id).Max(3))
            //        );

            //    //得到滚动扫描的id
            //    scrollid = result.ScrollId;
            //    foreach (var info in result.Documents)
            //    {
            //        Console.WriteLine(info.Id + " - " + " -批次count " + result.Documents.Count + " - 线程" + Thread.CurrentThread.ManagedThreadId);
            //    }
            //    while (true)
            //    {
            //        //执行滚动扫描得到数据 返回数据量是 result.Shards.Successful*size(查询成功的分片数*size)
            //        var result1 = elasticClient.Scroll<T>("1m", scrollid);
            //        if (result1.Documents == null || !result1.Documents.Any())
            //            break;
            //        foreach (var info in result1.Documents)
            //        {
            //            Console.WriteLine(info.Id + " - " + " -批次count " + result1.Documents.Count + " - 线程" + Thread.CurrentThread.ManagedThreadId);
            //        }
            //        //得到新的id
            //        scrollid = result1.ScrollId;
            //    }
            //};
            //var t1 = Task.Factory.StartNew(() => { sc1(0); });
            //var t2 = Task.Factory.StartNew(() => { sc1(1); });
            //var t3 = Task.Factory.StartNew(() => { sc1(2); });
            //t1.Wait();
            //t2.Wait();
            //t3.Wait();
        }






        //public void Search(string indexName)
        //{
        //    var result = _client.Search<TestModel5>(
        //        s => s
        //            .Index(indexName)//索引
        //            .Type(typeof(TestModel5))//类型
        //            .Explain() //参数可以提供查询的更多详情。
        //            .FielddataFields(fs => fs //对指定字段进行分析
        //                .Field(p => p.Name)
        //                .Field(p => p.Dic)
        //            )
        //            .From(0) //跳过的数据个数
        //            .Size(50) //返回数据个数
        //            .Query(q =>
        //                q.Term(p => p.State, 100) // 主要用于精确匹配哪些值，比如数字，日期，布尔值或 not_analyzed的字符串(未经分析的文本数据类型)：
        //                &&
        //                q.Term(p => p.Name.Suffix("temp"), "姓名") //用于自定义属性的查询 
        //                &&
        //                q.Bool( //bool 查询
        //                    b => b
        //                        //must  should  mushnot
        //                        .Must(mt => mt //所有分句必须全部匹配，与 AND 相同
        //                            .TermRange(p => p.Field(f => f.State).GreaterThan("0").LessThan("1"))) //指定范围查找
        //                        .Should(sd => sd //至少有一个分句匹配，与 OR 相同
        //                            .Term(p => p.State, 32915),
        //                            sd => sd.Terms(t => t.Field(fd => fd.State).Terms(new[] { 10, 20, 30 })),
        //                            //多值
        //                            //||
        //                            //sd.Term(p => p.priceID, 1001)
        //                            //||
        //                            //sd.Term(p => p.priceID, 1005)
        //                            sd => sd.TermRange(tr => tr.GreaterThan("10").LessThan("12").Field(f => f.State)),
        //                            //出入的时间必须指明时区
        //                            sd => sd.DateRange(tr => tr.GreaterThan(DateTime.Now.AddDays(-1)).LessThan(DateTime.Now).Field(f => f.CreateTime))

        //                        )
        //                        .MustNot(mn => mn//所有分句都必须不匹配，与 NOT 相同
        //                            .Term(p => p.State, 1001)
        //                            ,
        //                            mn => mn.Bool(
        //                                bb => bb.Must(mt => mt
        //                                      .Match(mc => mc.Field(fd => fd.Name).Query("至尊"))
        //                                ))
        //                        )
        //                    )
        //            )//查询条件
        //        .Sort(st => st.Ascending(asc => asc.Id))//排序
        //        //返回特定的字段
        //        //todo:5.x是sc.Includes
        //        .Source(sc => sc.Includes(ic => ic
        //            .Fields(
        //                fd => fd.Name,
        //                fd => fd.Id,
        //                fd => fd.CreateTime)))
        //       );
        //}




        public void Search<T>(string indexName, 
            Func<FieldsDescriptor<T>, IPromise<Fields>> func, 
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> selector,
            int From = 0,int Size= 50,
            params Expression<Func<T, object>>[] fields
          
            ) where T : class
        {



            //var result = _client.Search<TestModel5>(
            //              s => s
            //                  .Index(indexName)//索引
            //                  .Type(typeof(TestModel5))//类型
            //                  .Explain() //参数可以提供查询的更多详情。
            //                  .FielddataFields(fs => fs //对指定字段进行分析
            //                      .Field(p => p.Name)
            //                      .Field(p => p.Dic)
            //                  )
            //                  .From(0) //跳过的数据个数
            //                  .Size(50) //返回数据个数
            //                  .Query(q =>
            //                      q.Term(p => p.State, 100) // 主要用于精确匹配哪些值，比如数字，日期，布尔值或 not_analyzed的字符串(未经分析的文本数据类型)：
            //                      &&
            //                      q.Term(p => p.Name.Suffix("temp"), "姓名") //用于自定义属性的查询 
            //                      &&
            //                      q.Bool( //bool 查询
            //                          b => b
            //                              //must  should  mushnot
            //                              .Must(mt => mt //所有分句必须全部匹配，与 AND 相同
            //                                  .TermRange(p => p.Field(f => f.State).GreaterThan("0").LessThan("1"))) //指定范围查找
            //                              .Should(sd => sd //至少有一个分句匹配，与 OR 相同
            //                                  .Term(p => p.State, 32915),
            //                                  sd => sd.Terms(t => t.Field(fd => fd.State).Terms(new[] { 10, 20, 30 })),
            //                                  //多值
            //                                  //||
            //                                  //sd.Term(p => p.priceID, 1001)
            //                                  //||
            //                                  //sd.Term(p => p.priceID, 1005)
            //                                  sd => sd.TermRange(tr => tr.GreaterThan("10").LessThan("12").Field(f => f.State)),
            //                                  //出入的时间必须指明时区
            //                                  sd => sd.DateRange(tr => tr.GreaterThan(DateTime.Now.AddDays(-1)).LessThan(DateTime.Now).Field(f => f.CreateTime))

            //                              )
            //                              .MustNot(mn => mn//所有分句都必须不匹配，与 NOT 相同
            //                                  .Term(p => p.State, 1001)
            //                                  ,
            //                                  mn => mn.Bool(
            //                                      bb => bb.Must(mt => mt
            //                                            .Match(mc => mc.Field(fd => fd.Name).Query("至尊"))
            //                                      ))
            //                              )
            //                          )
            //                  )//查询条件
            //              .Sort(st => st.Ascending(asc => asc.Id))//排序
            //                                                      //返回特定的字段
            //                                                      //todo:5.x是sc.Includes
            //              .Source(sc => sc.Includes(ic => ic
            //                  .Fields(
            //                      fd => fd.Name,
            //                      fd => fd.Id,
            //                      fd => fd.CreateTime)))
            //             );







            var result = elasticClient.Search<T>(
                s => s
                    .Index(indexName.ToLower())                           //索引
                    .SearchType(SearchType.DfsQueryThenFetch)             //类型
                    .Explain()                                            //参数可以提供查询的更多详情。
                    .DocValueFields(func)
                    .From(0)                                              //跳过的数据个数
                    .Size(50)                                             //返回数据个数
                    .Query(query)                                         //查询条件
                    .Sort(selector)                                       //排序
                    .Source(sc => sc.Includes(ic => ic.Fields(fields)))   //返回特定的字段
               );
        }



        public void SearchSetBoost<T>(string indexName, Func<QueryContainerDescriptor<T>, QueryContainer> query,int Size=3000) where T : class
        {


            //// 在原分值基础上 设置不同匹配的加成值 具体算法为lucene内部算法
            //var result = _client.Search<TestModel5>(s => s
            //                .Index(indexName)
            //                .Query(q =>
            //                    q.Term(t => t
            //                                .Field(f => f.State).Value(2).Boost(4))
            //                                 ||
            //                    q.Term(t => t
            //                                .Field(f => f.State).Value(3).Boost(1))
            //                    )
            //                .Size(3000)
            //                .Sort(st => st.Descending(SortSpecialField.Score))
            //                );
            ////结果中state等于4的得分高



            // 在原分值基础上 设置不同匹配的加成值 具体算法为lucene内部算法
            var result = elasticClient.Search<T>(s => s
                            .Index(indexName.ToLower())
                            .Query(query)
                            .Size(Size)
                            .Sort(st => st.Descending(SortSpecialField.Score))
                            );
            //结果中state等于4的得分高
        }





        public void SearchAfter<T>(string indexName, 
            Func<QueryContainerDescriptor<T>,QueryContainer> query, 
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> selector
            ) where T : class
        {
            var response = elasticClient.Search<T>(s => s.Index(indexName).Query(query)
                 .Size(1000)
                 .Sort(selector)
                 .SearchAfter(new object[]
                 {
                   50,//上一次结果排序的最后ID值
                      //可以是多个排序字段的值
                 })
               );
        }



        public void ManyWhereSearch<T>(string indexName, 
            List<Func<QueryContainerDescriptor<T>, QueryContainer>> mustQuerys,
            List<Func<QueryContainerDescriptor<T>, QueryContainer>> shouldQuerys,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDesc
            ) where T : class
        {
            //bool useStateDesc = true;

            ////must 条件
            //var mustQuerys = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
            ////Deleted
            //mustQuerys.Add(mt => mt.Term(tm => tm.Field(fd => fd.Deleted).Value(false)));
            ////CreateTime
            //mustQuerys.Add(mt => mt.DateRange(tm => tm.Field(fd => fd.CreateTime).GreaterThanOrEquals(DateTime.Now.AddDays(-1)).LessThanOrEquals(DateTime.Now)));
          
            //should 条件
            //var shouldQuerys = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
            ////state
            //shouldQuerys.Add(mt => mt.Term(tm => tm.Field(fd => fd.State).Value(1)));
            //shouldQuerys.Add(mt => mt.Term(tm => tm.Field(fd => fd.State).Value(2)));

            //排序
            //Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDesc = sd =>
            //{
            //    //根据分值排序
            //    sd.Descending(SortSpecialField.Score);

            //    //排序
            //    if (useStateDesc)
            //        sd.Descending(d => d.State);
            //    else
            //        sd.Descending(d => d.Id);
            //    return sd;
            //};

            var result2 = elasticClient.Search<T>(s => s
                        .Index(indexName)
                        .Query(q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)))
                        .Size(100)
                        .From(0)
                        .Sort(sortDesc)
                   );
        }


        public void FunctionScore<T>(string indexName,
            Func<QueryContainerDescriptor<T>, QueryContainer> selector,
            Func<ScoreFunctionsDescriptor<T>, IPromise<IList<IScoreFunction>>> functions
            ) where T : class
        {

            //var result1 = _client.Search<TestModel5>(s => s
            //    .Index(indexName)
            //                    .Query(q => q.FunctionScore(f => f
            //                              //查询区
            //                              .Query(qq => qq.Term(t => t.Field(fd => fd.State).Value(1))
            //                                              ||
            //                                           qq.Term(t => t.Field(fd => fd.State).Value(2))
            //                              )
            //                              .Boost(1.0) //functionscore 对分值影响
            //                              .BoostMode(FunctionBoostMode.Replace)//计算boost 模式 ；Replace为替换
            //                              .ScoreMode(FunctionScoreMode.Sum) //计算score 模式；Sum为累加
            //                                                                //逻辑区
            //                              .Functions(fun => fun
            //                                  .Weight(w => w.Weight(3).Filter(ft => ft
            //                                      .Term(t => t.Field(fd => fd.State).Value(1))))//匹配cityid +3
            //                                  .Weight(w => w.Weight(2).Filter(ft => ft
            //                                      .Term(t => t.Field(fd => fd.State).Value(2))))//匹配pvcid +2
            //                              )
            //                        )
            //                       )
            //                    .Size(3000)
            //                    .Sort(st => st.Descending(SortSpecialField.Score))
            //                    );





            //使用functionscore计算得分
            var results = elasticClient.Search<T>(s => s
                            .Index(indexName.ToLower())
                            .Query(q => q.FunctionScore(f => f
                                                      //查询区
                                                      .Query(selector)
                                                      .Boost(1.0)                           //functionscore 对分值影响
                                                      .BoostMode(FunctionBoostMode.Replace) //计算boost 模式 ；Replace为替换
                                                      .ScoreMode(FunctionScoreMode.Sum)     //计算score 模式；Sum为累加
                                                      .Functions(functions)                 //逻辑区
                                                     )
                                )
                            .Size(3000)
                            .Sort(st => st.Descending(SortSpecialField.Score))
                            );
            //结果中 State=1，得分=3； State=2 ，得分=2 ,两者都满足的，得分=5
        }



        public void BaseAggregation<T>(string indexName,
            Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregationsSelector
            ) where T : class
        {
            var result = elasticClient.Search<T>(s => s
                .Index(indexName.ToLower())
                .From(0)
                .Size(15)
                .Aggregations(aggregationsSelector

                        //ag => ag
                        //    .ValueCount("Count", vc => vc.Field(fd => fd.Id))//总数
                        //    .Sum("vendorPrice_Sum", su => su.Field(fd => fd.Id))//求和
                        //    .Max("vendorPrice_Max", m => m.Field(fd => fd.Id))//最大值
                        //    .Min("vendorPrice_Min", m => m.Field(fd => fd.Id))//最小值
                        //    .Average("vendorPrice_Avg", avg => avg.Field(fd => fd.Id))//平均值
                        //    .Terms("vendorID_group", t => t.Field(fd => fd.Id).Size(100))//分组
                    )
                );


        }



        public void Aggregations<T>(string indexName) where T : class
        {
            //var result = elasticClient.Search<T>(s => s
            //.Index(indexName)
            //    .Size(0)
            //    .Aggregations(ag => ag
            //        .Terms("Group_group", //Group 分组
            //            t => t.Field(fd => fd.Group)
            //            .Size(100)
            //            .Aggregations(agg => agg
            //                            .Terms("Group_state_group", //Group_state
            //                                tt => tt.Field(fd => fd.State)
            //                                .Size(50)
            //                                .Aggregations(aggg => aggg
            //                                    .Average("g_g_Avg", av => av.Field(fd => fd.Dvalue))//Price avg
            //                                    .Max("g_g_Max", m => m.Field(fd => fd.Dvalue))//Price max
            //                                    .Min("g_g_Min", m => m.Field(fd => fd.Dvalue))//Price min
            //                                    .ValueCount("g_g_Count", m => m.Field(fd => fd.Id))//总记录数
            //                                    )
            //                                )
            //                            .Cardinality("g_count", dy => dy.Field(fd => fd.State))//分组数量
            //                            .ValueCount("g_Count", c => c.Field(fd => fd.Id))
            //                )
            //            )
            //            .Cardinality("vendorID_group_count", dy => dy.Field(fd => fd.Group))//分组数量
            //            .ValueCount("Count", c => c.Field(fd => fd.Id))//总记录数
            //    ) //分组
            //    );
        }

        public void AnalysisAggregationsResult<T>(string indexName) where T : class
        {
            //var mustQuerys = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();

            //mustQuerys.Add(t => t.Term(f => f.Deleted, false));

            //var result =
            //    elasticClient.Search<T>(
            //        s => s.Index(indexName)
            //            .Query(q => q
            //                    .Bool(b => b.Must(mustQuerys))
            //            )
            //            .Size(0)
            //            .Aggregations(ag => ag

            //                        .Terms("Group_Group", tm => tm
            //                                                    .OrderDescending("Dvalue_avg")//使用平均值排序 desc
            //                                                    .Field(fd => fd.Group)
            //                                                    .Size(100)
            //                                                    .Aggregations(agg => agg
            //                                                        .TopHits("top_test_hits", th => th.Sort(srt => srt.Field(fd => fd.Dvalue).Descending()).Size(1))//取出该分组下按dvalue分组
            //                                                        .Max("Dvalue_Max", m => m.Field(fd => fd.Dvalue))
            //                                                        .Min("Dvalue_Min", m => m.Field(fd => fd.Dvalue))
            //                                                        .Average("Dvalue_avg", avg => avg.Field(fd => fd.Dvalue))//平均值
            //                                                        )

            //                                                    )


            //                    )
            //            );
            //var vendorIdGroup = (BucketAggregate)result.Aggregations["Group_Group"];
            //foreach (var bucket1 in vendorIdGroup.Items)
            //{
            //    //todo:2.x KeyedBucket<T> 有泛型参数
            //    var bucket = (KeyedBucket<object>)bucket1;
            //    var maxPrice = ((ValueAggregate)bucket.Aggregations["Dvalue_Max"]).Value;
            //    var minPrice = ((ValueAggregate)bucket.Aggregations["Dvalue_Min"]).Value;
            //    var sources = ((TopHitsAggregate)bucket.Aggregations["top_test_hits"]).Documents<TestModel5>().ToList();
            //    var data = sources.FirstOrDefault();
            //}
        }


        /// <summary>
        /// null 值查询
        /// 当数据为Null时字段不存在
        /// </summary>
        /// <param name="indexName"></param>
        public void NullValueQuery<T>(string indexName,
            Func<ExistsQueryDescriptor<T>, IExistsQuery> selector
            ) where T : class
        {
            var result = elasticClient.Search<T>(
               s => s
                   .Index(indexName.ToLower())//索引
                   .SearchType(SearchType.QueryThenFetch)//类型
                                                         //selector=ex => ex.Field(fd => fd.Name)
                   .Query(q => q.Bool(b => b.Must(mt => mt.Exists(selector)))
                   ));

        }
        /// <summary>
        /// 空字符查询
        /// </summary>
        /// <param name="indexName"></param>
        public void StringEmptyQuery<T>(string indexName,
            Func<TermQueryDescriptor<T>, ITermQuery> selector

            ) where T : class
        {
            var result = elasticClient.Search<T>(
              s => s
                  .Index(indexName.ToLower())//索引
                  .SearchType(SearchType.QueryThenFetch)//类型            
                                                        //selector=ex => ex.Verbatim().Field(fd => fd.Name).Value("")
                  .Query(q => q.Bool(b => b.Must(mt => mt.Term(selector)))
                  ));
        }

    }
}
