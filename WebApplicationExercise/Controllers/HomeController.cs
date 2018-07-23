using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationExercise.Core;

namespace WebApplicationExercise.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        public ActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
