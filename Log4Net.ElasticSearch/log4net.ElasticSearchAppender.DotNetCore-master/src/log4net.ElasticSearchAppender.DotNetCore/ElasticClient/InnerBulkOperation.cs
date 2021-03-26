using System.Collections.Generic;

namespace log4net.ElasticSearchAppender.DotNetCore.ElasticClient
{
    public class InnerBulkOperation 
    {
        public string IndexName { get; set; }
        public string IndexType { get; set; }
        public object Document { get; set; }
        public Dictionary<string, string> IndexOperationParams { get; set; }

        public InnerBulkOperation()
        {
        }
    }
}
