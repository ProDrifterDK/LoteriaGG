using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGGNew.Controllers
{
    public class ChooseController : Controller
    {
        public ActionResult Index(string mensaje)
        {
            ViewBag.mensaje = mensaje;
            return View();
        }
    }
}