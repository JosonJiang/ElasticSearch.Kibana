using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Net.Monitor.Helper
{
    public enum HttpContextCategory { 
    
        Request,
        Response
    
    }
    public class HttpContextClient
    {
        #region 获取IP
        /// <summary>
        /// 获取Ip
        /// </summary>
        public static string IP
        {
            get
            {
                var result = string.Empty;
                if (HttpContext.Current != null)
                    result = GetWebClientIP();
                if (string.IsNullOrWhiteSpace(result))
                    result = GetLanIp();
                return result;
            }
        }
        /// <summary>
        /// 获取Web客户端的Ip
        /// </summary>
        /// <returns></returns>
        private static string GetWebClientIP()
        {
            var ip = GetWebRemoteIp();
            foreach (var hostAddress in Dns.GetHostAddresses(ip))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    return hostAddress.ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取Web远程Ip
        /// 没有使用代理服务器的情况
        /// </summary>
        /// <returns></returns>
        private static string GetWebRemoteIp()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }


        /// <summary>
        /// 获取局域网IP
        /// </summary>
        /// <returns></returns>
        private static string GetLanIp()
        {
            string ip = string.Empty;
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    //return hostAddress.ToString();
                    ip = ip + "|" + hostAddress.ToString();
            }
            if (!string.IsNullOrEmpty(ip) && ip.Contains("|"))
            {
                ip = ip.Substring(1);
            }
            return ip;
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP
        {
            get
            {
                string ClientIP = string.Empty;
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                    ClientIP = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                if (string.IsNullOrEmpty(ClientIP))
                    ClientIP = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                return ClientIP;
            }
        }


        /// <summary>
        /// 获取 User IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientALLIP()
        {

            var ClientProyxIP = HttpContext.Current.Request.ServerVariables["HTTP_VIA"];
            var ClientLastProyxIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            var ClientLastProyxIPOrRealIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var HttpClientIP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            var UserHostAddress = HttpContext.Current.Request.UserHostAddress;

            //一、没有使用代理服务器的情况：
            //      REMOTE_ADDR = 用户的 IP
            //      HTTP_VIA = 没数值或不显示
            //      HTTP_X_FORWARDED_FOR = 没数值或不显示

            //二、使用透明代理服务器的情况：Transparent Proxies
            //      REMOTE_ADDR = 最后一个代理服务器 IP
            //      HTTP_VIA = 代理服务器IP
            //      HTTP_X_FORWARDED_FOR = 用户的真实 IP ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

            //   这类代理服务器还是将您的信息转发给您的访问对象，无法达到隐藏真实身份的目的。

            //三、使用普通匿名代理服务器的情况：Anonymous Proxies
            //      REMOTE_ADDR = 最后一个代理服务器 IP
            //      HTTP_VIA = 代理服务器IP
            //      HTTP_X_FORWARDED_FOR = 代理服务器IP ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

            //   隐藏了您的真实IP，但是向访问对象透露了您是使用代理服务器访问他们的。

            //四、使用欺骗性代理服务器的情况：Distorting Proxies
            //      REMOTE_ADDR = 代理服务器 IP
            //      HTTP_VIA = 代理服务器 IP
            //      HTTP_X_FORWARDED_FOR = 随机的 IP ，经过多个代理服务器时，这个值类似如下：203.98.182.163, 203.98.182.163, 203.129.72.215。

            //   告诉了访问对象您使用了代理服务器，但编造了一个虚假的随机IP代替您的真实IP欺骗它。


            //到此总结 结论如下：
            //一、Request.ServerVariables["REMOTE_ADDR"]：的值始终等于 Request.UserHostAddress
            //二、Request.ServerVariables["HTTP_CLIENT_IP"]：的值始终等于空。
            //三、Request.ServerVariables["HTTP_VIA"]：的值就是CDN商。
            //四、Request.ServerVariables["HTTP_X_FORWARDED_FOR"]:为代理IP，多层代理将有多个IP，最前面为原始IP。


            string ClientIP = string.Empty;
            if (string.IsNullOrEmpty(ClientProyxIP))
            {
                //没有使用代理服务器
                ClientIP = ClientLastProyxIPOrRealIP;
            }
            else
            {
                ClientIP = $"{UserHostAddress}|{ClientLastProyxIP}|{ClientLastProyxIPOrRealIP}|{ClientProyxIP}|{HttpClientIP}";
            }
            return ClientIP;

        }






        #endregion

        #region Host(获取主机名)
        /// <summary>
        /// 获取主机名
        /// </summary>
        public static string Host
        {
            get
            {
                return HttpContext.Current == null ? Dns.GetHostName() : GetWebClientHostName();
            }
        }
        /// <summary>
        /// 获取Web客户端主机名
        /// </summary>
        /// <returns></returns>
        private static string GetWebClientHostName()
        {
            if (!HttpContext.Current.Request.IsLocal)
                return string.Empty;
            var ip = GetWebRemoteIp();
            var result = Dns.GetHostEntry(IPAddress.Parse(ip)).HostName;
            if (result == "localhost.localdomain")
                result = Dns.GetHostName();
            return result;
        }


        #endregion

        #region Browser(获取浏览器信息)
        /// <summary>
        /// 获取浏览器信息
        /// </summary>
        public static string Browser
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var browser = HttpContext.Current.Request.Browser;
                return string.Format("{0}|{1}", browser.Browser, browser.Version);
            }
        }

        #endregion

        public static string UserRequest
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;

                var UserRequest = HttpContext.Current.Request;
                var UserHttpRequest = String.Empty;

                foreach (String o in UserRequest.ServerVariables)
                {
                    UserHttpRequest += HttpContext.Current.Request.ServerVariables[o] + "|\n\r";
                }

                return UserHttpRequest;
            }
        }

    }
}


