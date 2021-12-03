using System.Web;
using System.Web.Mvc;
using Net.Monitor.Helper;
namespace Net.Framerwork.AdoNetAppenders
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Log4NetFilterAttribute());

        }
    }
}
