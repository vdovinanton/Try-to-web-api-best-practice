using System.Web.Mvc;

namespace WebApplicationExercise.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
