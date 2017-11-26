using LoteriaGG.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGGNew.Areas.LoL.Controllers
{
    public class SorteoController : BaseController
    {
        // GET: LoL/Sorteo
        public ActionResult Index()
        {
            if (UsuarioLogged == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

       
    }
}