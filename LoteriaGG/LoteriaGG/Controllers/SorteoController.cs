using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;

namespace LoteriaGG.Controllers
{
    public class SorteoController : Controller
    {
        // GET: Sorteo
        public ActionResult Index(string msj = null)
        {
            if(Session["LogedIn"]  == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            if(msj != null)
            {
                ViewBag.Mensaje = msj;
            }

            ViewBag.Sorteos = N_Sorteo.Sorteos();
            return View();
        }

        public ActionResult IngresarSorteo()
        {
            var d = Session["User"].ToString();
            string inscripcion = N_Sorteo.Inscripcion(d);

            return RedirectToAction("Index", new { msj = inscripcion });
        }
    }
}