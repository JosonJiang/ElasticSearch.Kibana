namespace log4net.ElasticSearchAppender.DotNetCore.Configuration
{
    public class IndexOperationParam
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public IndexOperationParam()
        {
        }

        public IndexOperationParam(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
