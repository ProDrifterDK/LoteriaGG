using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;

namespace LoteriaGG.Administrador.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: Administrador
        public ActionResult Index()
        {
            var user = Session["User"]?.ToString();
            if(user == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var usuAdm = N_Admin.IsAdmin(user);
            if (usuAdm == -1){
                return RedirectToAction("Index", "Home", new { });
            }

            Session["adm"] = usuAdm;   

            return View();
        }

        [HttpPost]
        public ActionResult Index(string fin)
        {
            fin = fin.Replace(" ", "-2016 ") + ":00";
            var d = N_Admin.IniciarToreneo(long.Parse(Session["adm"].ToString()), fin);
            if (d)
                ViewBag.Mensaje = "TOO BIEN XUXETUMARE :D";
            else
                ViewBag.Mensaje = "NO PA NA LOQUITO, PREGUNTALE AL DANIEL Q WA PAZÓ";
            return View();
        }   
    }
}