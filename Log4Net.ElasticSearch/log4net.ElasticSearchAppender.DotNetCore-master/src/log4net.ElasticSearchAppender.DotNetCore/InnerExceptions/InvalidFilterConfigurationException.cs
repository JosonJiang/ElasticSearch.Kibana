using System;

namespace log4net.ElasticSearchAppender.DotNetCore.InnerExceptions
{
    public class InvalidFilterConfigurationException : Exception
    {
        public InvalidFilterConfigurationException()
        {
        }

        public InvalidFilterConfigurationException(string message)
            : base(message)
        {
        }
    }
}
