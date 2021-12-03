using System;
using System.Collections.Generic;
using System.Text;

namespace NET.Standard.Elastic.AdoNetAppender
{
 

    #region 数据库日志相关
    /// <summary>
    /// 将数据库链接字符串独立出来
    /// </summary>
    //public class JosonAdoNetAppender : AdoNetAppender
    //{
    //    //private static IJosonLog _Log;
    //    //private string _ConnectionStringName;
    //    //public string ConnectionStringName
    //    //{
    //    //    get { return _ConnectionStringName; }
    //    //    set { _ConnectionStringName = value; }
    //    //}
    //    //protected static IJosonLog Log
    //    //{
    //    //    get
    //    //    {
    //    //        if (_Log == null)
    //    //            _Log = JosonLogManager.GetLogger(typeof(MyAdoNetAppender));
    //    //        return _Log;
    //    //    }
    //    //}
    //    //public override void ActivateOptions() { PopulateConnectionString(); base.ActivateOptions(); }

    //    ///// <summary>
    //    ///// 获取或设置数据库连接字符串
    //    ///// </summary>
    //    //private void PopulateConnectionString()
    //    //{
    //    //    // 如果配置文件中设置了ConnectionString，则返回
    //    //    if (!String.IsNullOrEmpty(ConnectionString)) return;
    //    //    // 如果配置文件中没有设置ConnectionStringName，则返回
    //    //    if (String.IsNullOrEmpty(ConnectionStringName)) return;
    //    //    // 获取对应Web.config中的连接字符串配置
    //    //    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
    //    //    if (settings == null)
    //    //    {
    //    //        if (Log.IsErrorEnabled)
    //    //            Log.ErrorFormat("Connection String Name not found in Configuration: {0}", ConnectionStringName);
    //    //        return;
    //    //    }
    //    //    //返回解密的连接字符串
    //    //    ConnectionString = settings.ConnectionString;// new DESEncrypt().Decrypt(settings.ConnectionString, null);
    //    //}
    //}
    #endregion

}
