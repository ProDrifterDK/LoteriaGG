using Datos.SqlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGG.Areas.Lol.Controllers
{
    public class WheelController : Controller
    {
        private readonly string[] options = { "1", "0", "0.1", "0.6", "0.1", "0.3", "0.4", "0.2", "0.4", "0.1", "0.2", "0.7", "0.1", "0.2", "0.2", "0.1", "0.9", "0" },
                                  options2 = { "5", "0", "0.3", "2", "0", "1", "0.5", "3", "0", "0.5", "0", "0.5", "1", "0.3", "0", "0.5", "1", "0.5", "0", "3", "0.3", "0.5", "1", "0.5", "0" };
        // GET: Lol/Weel
        public ActionResult Index()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "LoL" });
            }
            string user = Session["User"].ToString();
            using (var db = new LoteriaGGEntities())
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

        public ActionResult RecibirPremio(string val = "", int t = 0)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "LoL" });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult JsonSpin(int t)// Si t es 0 significa que no tiene daily free   
        {
            var user = Session["User"].ToString();
            using (var db = new LoteriaGGEntities())
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
                    usu.USU_SOR_DISP--;
                }
                db.TBL_USUARIO.Attach(usu);
                db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var rand = new Random();
                spinAngleStart = rand.NextDouble() * 10 + 10;
                spinTime = 0;
                spinTimeTotal = rand.NextDouble() * 3 + 4 * 1000;
                RotateWheel(t);
                ViewBag.GGCoins = usu.USU_SOR_DISP;

            }
            return Json(new { spinAngleStart = spinAngleStart, spinTimeTotal = spinTimeTotal }, JsonRequestBehavior.AllowGet);
        }

        int spinTime = 0;
        double startAngle = 0, spinAngleStart = 0, spinTimeTotal = 0;
        void RotateWheel(int t)
        {
            spinTime += 30;
            if (spinTime >= spinTimeTotal)
            {
                StopRotateWheel(t);
                return;
            }
            var spinAngle = spinAngleStart - EaseOut(spinTime, 0, spinAngleStart, spinTimeTotal);
            startAngle += (spinAngle * Math.PI / 180);
            RotateWheel(t);
        }

        void StopRotateWheel(int t)
        {
            var op = t != 1 ? options2 : options;
            var degrees = startAngle * 180 / Math.PI + 90;
            var arc = Math.PI / (op.Length / 2);
            var arcd = arc * 180 / Math.PI;
            int index = (int)Math.Floor((360 - degrees % 360) / arcd); //Index del arreglo
            while (index >= op.Length)
            {
                index -= op.Length;
            }
            var valor = float.Parse(op[index]); //Obtenemos el valor del arreglo opciones
            using (var db = new LoteriaGGEntities())
            {
                var user = Session["User"].ToString();
                var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);

                usu.USU_SOR_DISP += valor;

                db.TBL_USUARIO.Attach(usu);
                db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        double EaseOut(double t, int b, double c, double d)
        {
            var ts = (t /= d) * t;
            var tc = ts * t;
            return b + c * (tc + -3 * ts + 3 * t);
        }

    }
}