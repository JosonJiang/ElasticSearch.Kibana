using System.Collections.Generic;
using log4net.ElasticSearchAppender.DotNetCore.ElasticClient;

namespace log4net.ElasticSearchAppender.DotNetCore
{
    public interface IElasticAppenderFilter 
    {
        void PrepareConfiguration(IElasticsearchClient client);
        void PrepareEvent(Dictionary<string, object> logEvent);
    }
}
