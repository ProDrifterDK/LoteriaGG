using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;
using Datos.SqlData;
using System.Collections;

namespace LoteriaGG.Administrador.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: Administrador
        public ActionResult Index()
        {
            var user = Session["User"]?.ToString();
            if(user == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var usuAdm = N_Admin.IsAdmin(user);
            if (usuAdm == -1){
                return RedirectToAction("Index", "Home", new { });
            }

            Session["adm"] = usuAdm;   

            return View();
        }

        [HttpPost]
        public ActionResult Index(string fin)
        {
            var user = Session["User"]?.ToString();
            if (user == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var usuAdm = N_Admin.IsAdmin(user);
            if (usuAdm == -1)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            fin = fin.Replace(" ", "-2017 ") + ":00";
            var d = N_Admin.IniciarToreneo(long.Parse(Session["adm"].ToString()), fin);
            if (d)
                ViewBag.Mensaje = "TOO BIEN XUXETUMARE :D";
            else
                ViewBag.Mensaje = "NO PA NA LOQUITO, PREGUNTALE AL DANIEL Q WA PAZÓ";
            return View();
        }   
        public ActionResult SG()
        {
            var user = Session["User"]?.ToString();
            if (user == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var usuAdm = N_Admin.IsAdmin(user);
            if (usuAdm == -1)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            Session["adm"] = usuAdm;

            return View();
        }

        public JsonResult JsonSG()
        {
            var rtn = new ArrayList();

            using (var db = new LOTERIA_GGEntities())
            {
                var data = db.TBL_SORTEO_GRATIS.Where(o => o.SG_VALIDO).ToList();

                foreach (var item in data)
                {
                    rtn.Add(new
                    {
                        Codigo = item.SG_CODIGO.ToString(),
                        Copiar
                        =
                        "<a href=\"javascript:copyToClipboard('" + item.SG_CODIGO + "')\" data-toggle=\"modal\" class=\"on-default edit-row text-cetner glyphicon glyphicon-pencil\">Copiar</a>"
                    });
                }
            }

            return Json(new
            {
                data = rtn
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(int c)
        {
            var user = Session["User"]?.ToString();
            if (user == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var usuAdm = N_Admin.IsAdmin(user);
            if (usuAdm == -1)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            Session["adm"] = usuAdm;

            using (var db = new LOTERIA_GGEntities())
            {
                for (int i = 0; i < c; i++)
                {
                    var sg = new TBL_SORTEO_GRATIS();
                    sg.SG_CODIGO = Guid.NewGuid();
                    sg.SG_VALIDO = true;

                    db.TBL_SORTEO_GRATIS.Add(sg);
                }

                db.SaveChanges();
            }

            return View("SG");
        }
    }
}