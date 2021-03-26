using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net.ElasticSearchAppender.DotNetCore.Authentication;
using log4net.ElasticSearchAppender.DotNetCore.Configuration;

namespace log4net.ElasticSearchAppender.DotNetCore.ElasticClient
{
    public interface IElasticsearchClient : IDisposable
    {
        ServerDataCollection Servers { get; }
        bool Ssl { get; }
        bool AllowSelfSignedServerCert { get; }
        AuthenticationMethodChooser AuthenticationMethod { get; set; }
        void PutTemplateRaw(string templateName, string rawBody);
        void IndexBulk(IEnumerable<InnerBulkOperation> bulk);
        Task IndexBulkAsync(IEnumerable<InnerBulkOperation> bulk);
    }
}
