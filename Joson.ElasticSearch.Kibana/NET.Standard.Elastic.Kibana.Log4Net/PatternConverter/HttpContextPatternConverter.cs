using System;
using System.Collections.Generic;
using System.Text;

namespace NET.Standard.Elastic.HttpContextPatternConverter
{
    public class HttpContextPatternConverter
    {

    }

    public class HttpContextUserNameProvider
    {
        public override string ToString()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.User != null && context.User.Identity.IsAuthenticated)
            {
                return context.Identity.Name;
            }
            return "";
        }
    }



    public class HttpContextUserPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            string name = "";
            HttpContext context = HttpContext.Current;
            if (context != null && context.User != null && context.User.Identity.IsAuthenticated)
            {
                name = context.User.Identity.Name;
            }
            writer.Write(name);
        }
    }

    public class HttpContextSessionPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            //Use the value in Option as a key into HttpContext.Current.Session
            string setting = "";

            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                object sessionItem;
                sessionItem = context.Session[Option];
                if (sessionItem != null)
                {
                    setting = sessionItem.ToString();
                }
                writer.Write(setting);
            }
        }
    }

    class HttpContextItemPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            //Use the value in Option as a key into HttpContext.Current.Session
            string setting = "";

            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                object item;
                item = context.Items[Option];
                if (item != null)
                {
                    setting = item.ToString();
                }
                writer.Write(setting);
            }
        }
    }


}
