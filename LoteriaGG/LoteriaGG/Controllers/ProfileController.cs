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

        public ActionResult ObtenerSorteo(string msj = null)
        {
            if (msj != null)
            {
                ViewBag.Mensaje = msj;
            }

            return View();
        }

        [HttpPost]
        public ActionResult CambiarContrasena(string nContrasena, string nContrasenaR, string aContrasena)
        {
            //retornar vista diferente dependiendo de si es correcto o no *** IMPORTANTE!
            if (Session["LogedIn"] == null)
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

        [HttpPost]
        public ActionResult SorteoGratis(string codigo)
        {
            Guid Codigo;
            try
            {
                Codigo = Guid.Parse(codigo);
            }
            catch
            {
                return RedirectToAction("ObtenerSorteo", new { msj = "El codigo ingresado es incorrecto." });
            }
            using (var db = new LOTERIA_GGEntities())
            {
                var sg = db.TBL_SORTEO_GRATIS.FirstOrDefault(o => o.SG_CODIGO == Codigo && o.SG_VALIDO == true); 
                if(sg == null)
                {
                    return RedirectToAction("ObtenerSorteo", new { msj = "El codigo ingresado es incorrecto." });
                }
                else
                {
                    var usr = Session["User"].ToString();
                    var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usr);
                    if(usrD.USU_SOR_DISP == null)
                    {
                        usrD.USU_SOR_DISP = 0;
                    }
                    usrD.USU_SOR_DISP++;
                    sg.SG_VALIDO = false;
                    sg.USU_ID = usrD.USU_ID;
                    db.SaveChanges();
                    return RedirectToAction("ObtenerSorteo", new { msj = "Se ha cargado un sorteo a tu cuenta." });
                }
            }
        }
    }
}