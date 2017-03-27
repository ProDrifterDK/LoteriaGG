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
            if (!N_Admin.IsAdmin(user)){
                return RedirectToAction("Index", "Home", new { });
            }
            return View();
        }
    }
}