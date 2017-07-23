using Datos.SqlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGG.Areas.Steam.Controllers
{
    public class WheelController : Controller
    {
        // GET: Steam/Weel
        public ActionResult Index()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }
            var user = Session["User"];
            using (var db = new LOTERIA_GGEntities())
            {
                var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);
                if (usu.USU_DAILY_REWARD == null || usu.USU_DAILY_REWARD.Value.Day != DateTime.Now.Day)
                {
                    ViewBag.Spin = 1;
                }
            }
            return View();
        }

        public ActionResult RecibirPremio(string val = "")
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }

            var user = Session["User"];
            using (var db = new LOTERIA_GGEntities())
            {
                var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);
                if (usu.USU_DAILY_REWARD != null || usu.USU_DAILY_REWARD.Value.Day == DateTime.Now.Day)
                {
                    return RedirectToAction("Index", new { });
                }
            }

            return RedirectToAction("Index");
        }
    }
}