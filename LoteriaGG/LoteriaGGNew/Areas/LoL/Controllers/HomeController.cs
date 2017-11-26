using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;

namespace LoteriaGGNew.Areas.LoL.Controllers
{
    public class HomeController : Controller
    {
        // GET: LoL/Home
        public ActionResult Index()
        {
            ViewBag.Titulo = Home.Ganador(1);
            return View();
        }

        [HttpPost]
        public ActionResult Login(string argUsuario, string contrasena)
        {
            return RedirectToAction("Index");
        }

    }
}