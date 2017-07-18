using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGG.Areas.Steam.Controllers
{
    public class AboutController : Controller
    {
        // GET: Steam/About/
        public ActionResult Index()
        {
            return View();
        }
    }
}