using LoteriaGG.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGGNew.Areas.Steam.Controllers
{
    public class ObtenerGGCoinController : BaseController
    {
        // GET: Steam/ObtenerGGCoin
        public ActionResult Index(string mensaje = "", string exito = "")
        {
            ViewBag.mensaje = mensaje;
            ViewBag.exito = exito;

            if (UsuarioLogged == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}