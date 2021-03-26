using RestSharp;

namespace log4net.ElasticSearchAppender.DotNetCore.Authentication
{
    public class RequestData
    {
        public RestRequest RestRequest { get; set; }

        public string Url { get; set; }

        public string RequestString { get; set; }
    }
}
