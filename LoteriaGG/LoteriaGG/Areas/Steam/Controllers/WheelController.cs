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
            string user = Session["User"].ToString();
            using (var db = new LOTERIA_GGEntities())
            {
                var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);
                ViewBag.Spin = 0;
                ViewBag.GGCoins = usu.USU_SOR_DISP;
                if (usu.USU_DAILY_REWARD == null || usu.USU_DAILY_REWARD.Value.Day != DateTime.Now.Day)
                {
                    ViewBag.Spin = 1;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult RecibirPremio(string val = "", int t= 0)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }

            var user = Session["User"];
            using (var db = new LOTERIA_GGEntities())
            {
                var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);
                if (t == 1)
                {
                    if (val == "" || val == null)
                    {
                        return RedirectToAction("Index");
                    }

                    var valor = float.Parse(val);
                    usu.USU_SOR_DISP += valor;
                    usu.USU_SOR_DISP = Math.Round(usu.USU_SOR_DISP ?? 0, 1);

                    db.TBL_USUARIO.Attach(usu);
                    db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var valor = float.Parse(val);
                    usu.USU_SOR_DISP += valor;
                    usu.USU_SOR_DISP = Math.Round(usu.USU_SOR_DISP??0, 1);

                    db.TBL_USUARIO.Attach(usu);
                    db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult JsonSpin(int t)
        {
            var user = Session["User"];
            using(var db = new LOTERIA_GGEntities())
            {
                var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);
                if (t == 1)
                    usu.USU_DAILY_REWARD = DateTime.Now;
                else
                {
                    if (usu.USU_SOR_DISP < 1)
                    {
                        return RedirectToAction("Index");
                    }
                    usu.USU_SOR_DISP --;
                }

                db.TBL_USUARIO.Attach(usu);
                db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                ViewBag.GGCoins = usu.USU_SOR_DISP;

            }
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}