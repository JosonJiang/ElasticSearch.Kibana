using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NET.Standard.Elastic.PatternConverter
{
    using log4net;
    using log4net.Core;
    using log4net.Config;
    using log4net.Layout;
    using log4net.Layout.Pattern;


    /// <summary>
    /// 
    /// </summary>
    public class JosonLayout : CustomLayout
    {
        //参考文档 https://blog.csdn.net/fairylym/article/details/106253194
        //         https://blog.csdn.net/kongwei521/article/details/52242319
        //https://stackoverflow.com/questions/12139486/log4net-how-to-add-a-custom-field-to-my-logging
        public JosonLayout()
        {
            this.AddConverter("property", typeof(CustomerPatternConverter));
        }
    }

    public class CustomLayout : PatternLayout
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomLayout()
        {
            this.AddConverter("LogEntity", typeof(CustomerPatternConverter));
        }

    }


    #region CustomerPatternConverter
    public class CustomerPatternConverter : PatternLayoutConverter
    {

        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                // Write the value for the specified key
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }
        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>

        private object LookupProperty(string property, log4net.Core.LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            System.Reflection.PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }
    }

    #endregion

}
