using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;
using Datos.SqlData;
using System.Collections;

namespace LoteriaGG.Controllers
{
    public class SorteoController : Controller
    {
        // GET: Sorteo
        public ActionResult Index(string msj = null)
        {
            if(Session["LogedIn"]  == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            if(msj != null)
            {
                ViewBag.Mensaje = msj;
            }
            using(var db = new LOTERIA_GGEntities())
            {
                var d = Session["User"].ToString();

                var usr = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == d);

                ViewBag.Cantidad = usr.USU_SOR_DISP;
            }
            ViewBag.Sorteos = N_Sorteo.Sorteos();
            return View();
        }

        [HttpPost]
        public ActionResult Index(string sorID, string msj = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            var sId = long.Parse(sorID);

            msj = IngresarSorteo(sId);

            if (msj != null)
            {
                ViewBag.Mensaje = msj;
            }
            using (var db = new LOTERIA_GGEntities())
            {
                var d = Session["User"].ToString();

                var usr = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == d);

                ViewBag.Cantidad = usr.USU_SOR_DISP;
            }
            ViewBag.Sorteos = N_Sorteo.Sorteos();
            return View();
        }

        private string IngresarSorteo(long sorteo)
        {
            var d = Session["User"].ToString();
            
            return N_Sorteo.Inscripcion(d, sorteo);
        }

        public JsonResult JsonTabla()
        {
            var rtn = new ArrayList();

            using (var db = new LOTERIA_GGEntities())
            {
                var now = DateTime.Now.AddHours(1);
                var data = db.TBL_SORTEO.Where(o => o.SOR_FECHA_FIN.Value >= now).ToList();

                foreach (var item in data)
                {
                    rtn.Add(new { Id = "#"+item.SOR_ID.ToString(), FInicio = item.SOR_FECHA_INICIO.ToString(), FFin = item.SOR_FECHA_FIN.ToString() });
                }
            }

            return Json(new
            {
                data = rtn
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonTabla2()
        {
            var rtn = new ArrayList();

            using (var db = new LOTERIA_GGEntities())
            {
                var now = DateTime.Now.AddHours(1);
                var data = db.TBL_SORTEO.Where(o => o.SOR_FECHA_FIN.Value >= now).ToList();

                foreach (var item in data)
                {
                    rtn.Add(new
                    {
                        Id = "#" + item.SOR_ID.ToString(),
                        FInicio = item.SOR_FECHA_INICIO.ToString(),
                        FFin = item.SOR_FECHA_FIN.ToString(),
                        action = "<form action=\"/Sorteo/Index\" method=\"post\" role=\"form\" >" +
                        "<input type=\"hidden\" name=\"sorID\" value=\"" + item.SOR_ID + "\">" +
                        "<input type=\"submit\" value=\"Inscribirse\" style=\"border-color:rgba(92, 239, 192, 50);color:#5a1650;background-color:rgba(92, 239, 192, 50)\" />" +
                        "</form>"
                    });
                }
            }

            return Json(new
            {
                data = rtn
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Inscripcion(string msj = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            if (msj != null)
            {
                ViewBag.Mensaje = msj;
            }

            ViewBag.Sorteos = N_Sorteo.Sorteos();
            return View();
        }
    }
}