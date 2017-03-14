using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos.SqlData;
using Negocio;

namespace LoteriaGG.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            return View(us);
        }

        public ActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarContrasena(string nContrasena, string nContrasenaR, string aContrasena)
        {
            //retornar vista diferente dependiendo de si es correcto o no *** IMPORTANTE!
            if(Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var respuesta = Class1.CambiarContrasena(Session["User"].ToString(), nContrasena, nContrasenaR, aContrasena);

            ViewBag.cambio = respuesta;

            return View();
        }

        public ActionResult EditarPerfil()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            return View(us);
        }

        [HttpPost]
        public ActionResult EditarPerfil(string nombre, string apellido, string nombreInvocador)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            ViewBag.tr = Class1.EditarPerfil(Session["User"].ToString(), nombre, apellido, nombreInvocador);

            return View(us);
        }
        
        
    }
}