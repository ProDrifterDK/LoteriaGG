using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGGNew.Areas.Steam.Controllers
{
    public class HomeController : Controller
    {
        // GET: Steam/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}