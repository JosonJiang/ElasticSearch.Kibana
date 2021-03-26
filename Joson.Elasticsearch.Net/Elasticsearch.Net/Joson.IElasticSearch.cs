
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

    public static class IClients
    {
        #region CreateDocument

        //https://blog.csdn.net/weixin_44526839/article/details/106386260?spm=1001.2014.3001.5502

        /// <summary>
        /// Create Document
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="elasticClient"></param>
        /// <param name="indexName">索引名称</param>
        /// <param name="entity"></param>
        /// <param name="id">文档Id需为唯一标识。文档Id主要用途：1.修改 / 删除 数据时,需指定文档Id</param>
        /// <returns></returns>
        public static CreateResponse CreateDocument<TEntity>(this IElasticClient elasticClient
            , TEntity entity
            , string indexName
            )
            where TEntity : class
        {
            string idValue = ElasticsearchKit.GetIdPropertyValue(entity).ToStrAndTrim();
            CreateResponse response = elasticClient.Create<TEntity>(entity, t => t.Index(indexName).Id(idValue));
            return response;
        }

        #endregion

        #region UpdateDocument

        /// <summary>
        /// Update Document
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="elasticClient"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static UpdateResponse<TEntity> UpdateDocument<TEntity>(this IElasticClient elasticClient
            , TEntity entity
            )
            where TEntity : class
        {
            string idValue = ElasticsearchKit.GetIdPropertyValue(entity).ToStrAndTrim();
            // [ 全文更新 ]
            UpdateResponse<TEntity> response = elasticClient.Update<TEntity>(idValue, t => t.Doc(entity));
            return response;
        }

        /// <summary>
        /// 部分更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPartialEntity"></typeparam>
        /// <param name="elasticClient"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static UpdateResponse<TEntity> UpdateDocumentPartial<TEntity, TPartialEntity>(this IElasticClient elasticClient
            , IUpdateRequest<TEntity, TPartialEntity> request
            )
            where TEntity : class
            where TPartialEntity : class
        {
            //[  部分字段/局部更新 (TPartialEntity中的字段) ]
            UpdateResponse<TEntity> response = elasticClient.Update(request);
            return response;
        }


        /// <summary>
        /// 部分更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPartialEntity"></typeparam>
        /// <param name="elasticClient"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static UpdateResponse<TEntity> UpdateDocumentPartial<TEntity>(this IElasticClient elasticClient
            , TEntity entity
            , object partialEntity
            , string indexName = null
            )
            where TEntity : class
        {
            string idValue = ElasticsearchKit.GetIdPropertyValue(entity).ToStrAndTrim();
            IUpdateRequest<TEntity, object> request = new UpdateRequest<TEntity, object>(indexName, idValue)
            {
                Doc = partialEntity
            };

            //[  部分字段/局部更新 (TPartialEntity中的字段) ]
            UpdateResponse<TEntity> response = elasticClient.Update(request);
            return response;
        }

        #endregion

        #region DeleteDocument

        public static DeleteResponse DeleteDocument<TEntity>(this IElasticClient elasticClient
            , TEntity entity
            , string indexName
            )
        {
            string idValue = ElasticsearchKit.GetIdPropertyValue(entity).ToStrAndTrim();
            return DeleteDocument(elasticClient, indexName, idValue);
        }

        public static DeleteResponse DeleteDocument(this IElasticClient elasticClient
           , string id
            , string indexName
            )
        {
            DeleteRequest request = new DeleteRequest(indexName, id);
            DeleteResponse response = elasticClient.Delete(request);
            return response;
        }

        public static DeleteResponse DeleteDocument(this IElasticClient elasticClient
            , long id
            , string indexName
            )
        {
            DeleteRequest request = new DeleteRequest(indexName, id);
            DeleteResponse response = elasticClient.Delete(request);
            return response;
        }

        #endregion

        #region GetVersionDocument

        public static GetResponse<TEntity> GetVersion<TEntity>(this IElasticClient elasticClient
            , string id
            , string indexName
            ) where TEntity : class
        {
            DocumentPath<TEntity> path = new DocumentPath<TEntity>(id);
            var response = elasticClient.Get<TEntity>(path, s => s.Index(indexName));
            return response;// response.Version
        }

        public static GetResponse<TEntity> GetDocument<TEntity>(this IElasticClient elasticClient
      , string id
      , string indexName
      ) where TEntity : class
        {
            DocumentPath<TEntity> path = new DocumentPath<TEntity>(id);
            var response = elasticClient.Get<TEntity>(path, s => s.Index(indexName));
            return response;// response.Version
        }

        #endregion

        /// <summary>
        /// 获取IdProperty的名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetIdPropertyName<TEntity>(TEntity entity)
        {
            ElasticsearchTypeAttribute elasticsearchType = ReflectionKit.GetClassCustomAttribute<ElasticsearchTypeAttribute>(typeof(TEntity));
            if (elasticsearchType != null)
                return elasticsearchType.IdProperty;
            else
                throw new Exception("");
        }

        /// <summary>
        /// 获取 IdProperty 的 值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetIdPropertyValue<TEntity>(TEntity entity)
        {
            string idPropertyName = GetIdPropertyName(entity);
            if (String.IsNullOrWhiteSpace(idPropertyName))
            {
                Dictionary<string, object> keyValues = entity.GetProperty();
                keyValues.TryGetValue(idPropertyName, out object obj);
                return obj;
            }
            return null;
        }
        #region ExistsIndex
        /// <summary>
        /// 判断索引是否存在
        /// </summary>
        /// <param name="elasticClient"></param>
        /// <param name="indexName"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static bool ExistsIndex(this IElasticClient elasticClient
            , string indexName, Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null)
        {
            var existResponse = elasticClient.Indices.Exists(indexName, selector);
            return existResponse.Exists;
        }
        #endregion

        #region CreateIndex

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="elasticClient"></param>
        public static CreateIndexResponse CreateIndex(this IElasticClient elasticClient
            , string indexName
            , int numberOfReplicas = 1
            , int numberOfShards = 5
            )
        {
            IIndexState indexState = new IndexState
            {
                Settings = new IndexSettings
                {
                    NumberOfReplicas = numberOfReplicas, // [副本数量]
                    NumberOfShards = numberOfShards //[分片数量]
                }
            };
            Func<CreateIndexDescriptor, ICreateIndexRequest> func = x => x.InitializeUsing(indexState).Map(m => m.AutoMap());
            CreateIndexResponse response = elasticClient.Indices.Create(indexName, func);
            return response;
        }


        #endregion

        #region DeleteIndex
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="elasticClient"></param>
        /// <param name="indexName"></param>
        public static DeleteIndexResponse DeleteIndex(this IElasticClient elasticClient, string indexName)
        {
            DeleteIndexResponse response = elasticClient.Indices.Delete(indexName);
            return response;
        }

        #endregion

    }
}
