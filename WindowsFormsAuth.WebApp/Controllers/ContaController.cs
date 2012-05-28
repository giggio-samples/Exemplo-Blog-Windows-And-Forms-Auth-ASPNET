using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;

namespace WindowsFormsAuth.WebApp.Controllers
{
    public class ContaController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return !string.IsNullOrWhiteSpace(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Home");
            if (RequestVemDaExtranet())
                return RedirectToRoute("Default", new { action = "WebLogin", returnUrl = returnUrl });
            return RedirectToRoute("Default", new { action = "WindowsLogin", returnUrl = returnUrl });
        }

        public ActionResult WindowsLogin(string returnUrl)
        {
            var usuario = Request.ServerVariables["LOGON_USER"];

            if (string.IsNullOrWhiteSpace(usuario))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            FormsAuthentication.SetAuthCookie(usuario, true);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult WebLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WebLogin(string usuario, string senha, bool lembreDeMim, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WindowsAuthenticate(usuario, senha))
                {
                    FormsAuthentication.SetAuthCookie(usuario, lembreDeMim);
                    return !string.IsNullOrWhiteSpace(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Usuario = usuario;
            ViewBag.LembreDeMim = lembreDeMim;
            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool RequestVemDaExtranet()
        {
            return string.Compare(Request.Url.DnsSafeHost, "localhost", true) == 0;
        }

        private static bool WindowsAuthenticate(string usuario, string senha)
        {
            using (var context = new PrincipalContext(ContextType.Machine, "nomedasuamaquina"))
            {
                return context.ValidateCredentials(usuario, senha);
            }
        }
    }
}
