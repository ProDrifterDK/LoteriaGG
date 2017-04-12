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

            ViewBag.Sorteos = N_Sorteo.Sorteos();
            return View();
        }

        public ActionResult IngresarSorteo(int sorteo)
        {
            var d = Session["User"].ToString();
            string inscripcion = N_Sorteo.Inscripcion(d, sorteo);

            return RedirectToAction("Index", new { msj = inscripcion });
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
                        action =
                        "<form action=\"https://www.paypal.com/cgi-bin/webscr\" method=\"post\" target=\"_top\">"+
                        "< input type = \"hidden\" name = \"cmd\" value = \"_s-xclick\" >"+
                        "< input type = \"hidden\" name = \"sorteo\" value ="+ item.SOR_ID +">"+
                        "< input type = \"hidden\" name = \"hosted_button_id\" value = \"397G378AZVW4C\" >" +
                        "< input type = \"image\" src = \"https://www.paypalobjects.com/en_US/i/btn/btn_subscribe_LG.gif\" border = \"0\" name = \"submit\" alt = \"PayPal - The safer, easier way to pay online!\" >"+
                        "< img alt = \"\" border = \"0\" src = \"https://www.paypalobjects.com/es_XC/i/scr/pixel.gif\" width = \"1\" height = \"1\" >"+
                        "</ form >"
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