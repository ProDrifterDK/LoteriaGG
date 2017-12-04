using LoteriaGG.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGGNew.Areas.LoL.Controllers
{
    public class RuletaController : BaseController
    {
        // GET: LoL/Ruleta
        private readonly string[] options = { "1", "0", "0.1", "0.6", "0.1", "0.3", "0.4", "0.2", "0.4", "0.1", "0.2", "0.7", "0.1", "0.2", "0.2", "0.1", "0.9", "0" },
                                  options2 = { "5", "0", "0.3", "2", "0", "1", "0.5", "3", "0", "0.5", "0", "0.5", "1", "0.3", "0", "0.5", "1", "0.5", "0", "3", "0.3", "0.5", "1", "0.5", "0" };
        // GET: Lol/Weel
        public ActionResult Index()
        {
            ViewBag.Spin = 0;
            ViewBag.GGCoins = Math.Round(UsuarioLogged.USU_SOR_DISP ?? 0, 1);
            ViewBag.arc = Math.PI / (options2.Length / 2);
            if (UsuarioLogged.USU_DAILY_REWARD == null || UsuarioLogged.USU_DAILY_REWARD.Value.Day != DateTime.Now.Day)
            {
                ViewBag.Spin = 1;
                ViewBag.arc = Math.PI / (options.Length / 2);
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
            var usu = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);
            if (t == 1)
                usu.USU_DAILY_REWARD = DateTime.Now;
            else
            {
                if (usu.USU_SOR_DISP < 1)
                {
                    return Json(new { exito = false, mensaje = "No tienes GGCoins suficientes." }, JsonRequestBehavior.AllowGet);
                }
                usu.USU_SOR_DISP--;
            }
            BDD.TBL_USUARIO.Attach(usu);
            BDD.Entry(usu).State = System.Data.Entity.EntityState.Modified;
            BDD.SaveChanges();

            var rand = new Random();
            spinAngleStart = rand.NextDouble() * 10 + 10;
            spinTime = 0;
            spinTimeTotal = rand.NextDouble() * 3 + 4 * 1000;
            var ggCoinsGanados = RotateWheel(t);
            ViewBag.GGCoins = usu.USU_SOR_DISP;
            UsuarioLogged = usu;

            return Json(new { exito = true, spinAngleStart = spinAngleStart, spinTimeTotal = spinTimeTotal, GGCoinsGanados = ggCoinsGanados[0], GGCoins = usu.USU_SOR_DISP, arcd = ggCoinsGanados[1] }, JsonRequestBehavior.AllowGet);
        }

        int spinTime = 0;
        double startAngle = 0, spinAngleStart = 0, spinTimeTotal = 0;
        double [] RotateWheel(int t)
        {
            spinTime += 30;
            if (spinTime >= spinTimeTotal)
            {
                return StopRotateWheel(t);
            }
            var spinAngle = spinAngleStart - EaseOut(spinTime, 0, spinAngleStart, spinTimeTotal);
            startAngle += Math.Round(spinAngle * Math.PI / 180, 10);
            startAngle = Math.Round(startAngle, 10);
            return RotateWheel(t);
        }

        double [] StopRotateWheel(int t)
        {
            var op = t != 1 ? options2 : options;
            var degrees = Math.Round(startAngle * 180 / Math.PI + 90, 10);
            var arc = Math.PI / (op.Length / 2);
            var arcd = arc * 180 / Math.PI;
            int index = (int)Math.Floor((360 - degrees % 360) / arcd); //Index del arreglo
            while (index >= op.Length)
            {
                index -= op.Length;
            }
            var valor = double.Parse(op[index]); //Obtenemos el valor del arreglo opciones
            var usu = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);

            usu.USU_SOR_DISP = usu.USU_SOR_DISP + valor;
            usu.USU_SOR_DISP = Math.Round(usu.USU_SOR_DISP ?? 0, 1);

            BDD.TBL_USUARIO.Attach(usu);
            BDD.Entry(usu).State = System.Data.Entity.EntityState.Modified;
            BDD.SaveChanges();
            return new double [] { valor, arcd };
        }

        double EaseOut(double t, int b, double c, double d)
        {
            var ts = (t /= d) * t;
            var tc = ts * t;
            return b + c * (tc + -3 * ts + 3 * t);
        }
    }
}