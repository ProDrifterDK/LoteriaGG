using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGGNew.Areas.LoL.Controllers
{
    public class HomeController : Controller
    {
        // GET: LoL/Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string argUsuario, string contrasena)
        {
            return RedirectToAction("Index");
        }

    }
}