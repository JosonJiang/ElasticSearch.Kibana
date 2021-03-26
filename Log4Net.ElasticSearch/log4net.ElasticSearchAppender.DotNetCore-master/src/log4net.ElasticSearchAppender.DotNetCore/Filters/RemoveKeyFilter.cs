using System.Collections.Generic;
using log4net.ElasticSearchAppender.DotNetCore.ElasticClient;
using log4net.ElasticSearchAppender.DotNetCore.SmartFormatters;

namespace log4net.ElasticSearchAppender.DotNetCore.Filters
{
    public class RemoveKeyFilter : IElasticAppenderFilter
    {
        private LogEventSmartFormatter _key;

        [PropertyNotEmpty]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public void PrepareConfiguration(IElasticsearchClient client)
        {
        }

        public void PrepareEvent(Dictionary<string, object> logEvent)
        {
            logEvent.Remove(_key.Format(logEvent));
        }
    }
}
