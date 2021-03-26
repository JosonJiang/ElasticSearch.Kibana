
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Elasticsearch.Net;
using Nest;
namespace Joson.Elastic.Search
{
    /// <summary>
    /// https://www.shuzhiduo.com/A/l1dyoKrnJe/
    /// https://www.debugcn.com/article/48359184.html
    /// </summary>
    public static class Clients
    {
        /// <summary>
        /// 获取ElasticClient
        /// </summary>
        /// <param name="url">ElasticSearch服务器地址</param>
        /// <param name="defaultIndex">默认索引名称</param>
        /// <returns></returns>
        public static ElasticClient Client(string url, string defaultIndex = "JosonIndex")
        {
            var uri = new Uri(url);
            var setting = new ConnectionSettings(uri);

            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                setting.DefaultIndex(defaultIndex);
            }

            //string defaultType = "_doc";
            //if (!string.IsNullOrWhiteSpace(defaultType))
            //{
            //    setting.DefaultTypeName(defaultType);
            //}

            return new ElasticClient(setting);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="defaultIndex"></param>
        /// <returns></returns>
        public static ElasticClient Client(string url, string userName, string password, string defaultIndex = "JosonIndex")
        {
            var uri = new Uri($"http://{userName}:{password}@{url}");
            var settings = new ConnectionConfiguration(uri);

            var setting = new ConnectionSettings(uri);

            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                setting.DefaultIndex(defaultIndex);
            }

            return new ElasticClient(setting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="defaultIndex"></param>
        /// <returns></returns>
        public static ElasticClient Client(Uri url, string defaultIndex = "JosonIndex")
        {


            var setting = new ConnectionSettings(url);

            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                setting.DefaultIndex(defaultIndex);
            }
            return new ElasticClient(setting);
        }


        /// <summary>
        /// 获取ElasticClient
        /// </summary>
        /// <param name="urls">ElasticSearch集群地址</param>
        /// <param name="defaultIndex">默认索引名称</param>
        /// <returns></returns>
        public static ElasticClient Client(string[] urls, string defaultIndex = "JosonIndex")
        {
            var uris = urls.Select(h => new Uri(h)).ToArray();
            var pool = new SniffingConnectionPool(uris);

            var setting = new ConnectionSettings(pool);

            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                setting.DefaultIndex(defaultIndex);
            }



            return new ElasticClient(setting);
        }


        /// <summary>
        /// 获取ElasticClient
        /// </summary>
        /// <param name="urls">ElasticSearch集群地址</param>
        /// <param name="defaultIndex">默认索引名称</param>
        /// <returns></returns>
        public static ElasticClient Client(Uri[] urls, string defaultIndex = "JosonIndex")
        {

            //var uris = new[]{
            //                 new Uri("http://localhost:9200"),
            //                 new Uri("http://localhost:9201"),
            //                 new Uri("http://localhost:9202")
            //                };

            var connectionPool = new SniffingConnectionPool(urls);
            var setting = new ConnectionSettings(connectionPool);
            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                setting.DefaultIndex(defaultIndex);
            }

            return new ElasticClient(setting);

        }



        /// <summary>
        /// 如果同名索引不存在则创建索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client">ElasticClient实例</param>
        /// <param name="indexName">要创建的索引名称</param>
        /// <param name="numberOfReplicas">默认副本数量，如果是单实例，注意改成0</param>
        /// <param name="numberOfShards">默认分片数量</param>
        /// <returns></returns>
        public static bool CreateIndex<T>(this ElasticClient elasticClient, string indexName = "JosonIndex", int numberOfReplicas = 0, int numberOfShards = 2) where T : class
        {
            var result = false;
            bool exists = elasticClient.Indices.Exists(indexName).Exists;
            if (exists) return false;


            if (string.IsNullOrWhiteSpace(indexName))
            {
                indexName = typeof(T).Name.ToLower();
            }

            var indexState = new IndexState
            {
                Settings = new IndexSettings
                {
                    NumberOfReplicas = numberOfReplicas,    //副本数
                    NumberOfShards = numberOfShards         //分片数
                }
            };

            try
            {
                var response = elasticClient.Indices.Create(indexName, index => index.InitializeUsing(indexState).Map<T>(x => x.AutoMap()));
                result = response.Acknowledged;
            }
            catch (Exception ex)
            {
                result = elasticClient.CreateIndex<T>(indexName, numberOfReplicas, numberOfShards);
            }

            return result;
        }

        /// <summary>
        /// 返回一个正序排列的委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<SortDescriptor<T>, SortDescriptor<T>> Sort<T>(string field) where T : class
        {
            return sd => sd.Ascending(field);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<SortDescriptor<T>, SortDescriptor<T>> Sort<T>(Expression<Func<T, object>> field) where T : class
        {
            return sd => sd.Ascending(field);
        }

        /// <summary>
        /// 返回一个倒序排列的委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<SortDescriptor<T>, SortDescriptor<T>> SortDesc<T>(string field) where T : class
        {
            return sd => sd.Descending(field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<SortDescriptor<T>, SortDescriptor<T>> SortDesc<T>(Expression<Func<T, object>> field) where T : class
        {
            return sd => sd.Descending(field);
        }

        /// <summary>
        /// 返回一个Must条件集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Func<QueryContainerDescriptor<T>, QueryContainer>> Must<T>() where T : class
        {
            return new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Func<QueryContainerDescriptor<T>, QueryContainer>> Should<T>() where T : class
        {
            return new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
        }

        /// <summary>
        /// 添加Match子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要查询的关键字</param>
        /// <param name="boost"></param>
        public static void AddMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, string value, double? boost = null) where T : class
        {
            musts.Add(d => d.Match(mq => mq.Field(field).Query(value).Boost(boost)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void AddMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, string value) where T : class
        {
            musts.Add(d => d.Match(mq => mq.Field(field).Query(value)));
        }

        /// <summary>
        /// 添加MultiMatch子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="fields">要查询的列</param>
        /// <param name="value">要查询的关键字</param>
        public static void AddMultiMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string[] fields, string value) where T : class
        {
            musts.Add(d => d.MultiMatch(mq => mq.Fields(fields).Query(value)));
        }

        /// <summary>
        /// 添加MultiMatch子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="fields">例如：f=>new [] {f.xxx, f.xxx}</param>
        /// <param name="value">要查询的关键字</param>
        public static void AddMultiMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> fields, string value) where T : class
        {
            musts.Add(d => d.MultiMatch(mq => mq.Fields(fields).Query(value)));
        }

        /// <summary>
        /// 添加大于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddGreaterThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThan(value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void AddGreaterThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThan(value)));
        }

        /// <summary>
        /// 添加大于等于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddGreaterThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThanOrEquals(value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void AddGreaterThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThanOrEquals(value)));
        }

        /// <summary>
        /// 添加小于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddLessThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThan(value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void AddLessThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThan(value)));
        }

        /// <summary>
        /// 添加小于等于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddLessThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThanOrEquals(value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void AddLessThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThanOrEquals(value)));
        }

        /// <summary>
        /// 添加一个Term，一个列一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddTerm<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, object value) where T : class
        {
            musts.Add(d => d.Term(field, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void AddTerm<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, object value) where T : class
        {
            musts.Add(d => d.Term(field, value));
        }

        /// <summary>
        /// 添加一个Terms，一个列多个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        public static void AddTerms<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field, object[] values) where T : class
        {
            musts.Add(d => d.Terms(tq => tq.Field(field).Terms(values)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        public static void AddTerms<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, Expression<Func<T, object>> field, object[] values) where T : class
        {
            musts.Add(d => d.Terms(tq => tq.Field(field).Terms(values)));
        }
    }
}
