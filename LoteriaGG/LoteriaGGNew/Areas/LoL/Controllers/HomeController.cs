using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;
using LoteriaGG.Base;

namespace LoteriaGGNew.Areas.LoL.Controllers
{
    public class HomeController : BaseController
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
            var usuario = Class1.Login(argUsuario, contrasena);
            if (usuario == null)
            {
                return Json(new { data = "Usuario o contraseña incorrectos o no existentes." }, JsonRequestBehavior.AllowGet);
            }
            UsuarioLogged = usuario;
            return Json(new { data = "Exito" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

    }
}