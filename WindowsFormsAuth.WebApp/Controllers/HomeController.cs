using System.Net;
using System.Web.Mvc;

namespace WindowsFormsAuth.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Seguro()
        {
            return View();
        }
        public ActionResult TentandoPassar401()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }

      
    }
}
