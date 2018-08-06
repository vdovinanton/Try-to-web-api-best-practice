using System.Web.Mvc;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
