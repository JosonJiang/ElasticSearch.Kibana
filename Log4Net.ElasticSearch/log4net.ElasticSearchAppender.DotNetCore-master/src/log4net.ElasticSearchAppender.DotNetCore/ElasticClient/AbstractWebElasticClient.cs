using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using log4net.ElasticSearchAppender.DotNetCore.Authentication;
using log4net.ElasticSearchAppender.DotNetCore.Configuration;

namespace log4net.ElasticSearchAppender.DotNetCore.ElasticClient
{
    public abstract class AbstractWebElasticClient : IElasticsearchClient
    {
        public ServerDataCollection Servers { get; private set; }
        public int Timeout { get; private set; }
        public bool Ssl { get; private set; }
        public bool AllowSelfSignedServerCert { get; private set; }
        public AuthenticationMethodChooser AuthenticationMethod { get; set; }
        public string Url => GetServerUrl();

        protected AbstractWebElasticClient(ServerDataCollection servers,
            int timeout,
            bool ssl,
            bool allowSelfSignedServerCert,
            AuthenticationMethodChooser authenticationMethod)
        {
            Servers = servers;
            Timeout = timeout;
            ServicePointManager.Expect100Continue = false;

            // SSL related properties
            Ssl = ssl;
            AllowSelfSignedServerCert = allowSelfSignedServerCert;
            AuthenticationMethod = authenticationMethod;
        }

        public abstract void PutTemplateRaw(string templateName, string rawBody);
        public abstract void IndexBulk(IEnumerable<InnerBulkOperation> bulk);
        public abstract Task IndexBulkAsync(IEnumerable<InnerBulkOperation> bulk);
        public abstract void Dispose();

        protected string GetServerUrl()
        {
            var serverData = Servers.GetRandomServerData();
            var url = $"{(Ssl ? "https" : "http")}://{serverData.Address}:{serverData.Port}{(string.IsNullOrEmpty(serverData.Path) ? "" : serverData.Path)}/";
            return url;
        }

        protected string GetServerUrl(IServerData serverData)
        {
            var url = $"{(Ssl ? "https" : "http")}://{serverData.Address}:{serverData.Port}{(string.IsNullOrEmpty(serverData.Path) ? "" : serverData.Path)}/";
            return url;
        }
    }
}
