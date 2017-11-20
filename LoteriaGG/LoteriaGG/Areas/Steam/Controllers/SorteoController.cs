using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;
using Datos.SqlData;
using System.Collections;
using LoteriaGG.Base;

namespace LoteriaGG.Areas.Steam.Controllers
{
    public class SorteoController : BaseController
    {
        // GET: /Steam/Sorteo/
        public ActionResult Index(string msj = null)
        {
            if(Session["LogedIn"]  == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam"});
            }
            if(msj != null)
            {
                ViewBag.Mensaje = msj;
            }
            using(var db = new LoteriaGGEntities())
            {
                var d = Session["User"].ToString();

                var usr = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == d);

                ViewBag.Cantidad = usr.USU_SOR_DISP;
            }
            ViewBag.Sorteos = N_Sorteo.Sorteos();
            return View();
        }


        [HttpPost]
        public ActionResult Index(string sorID, string msj = null, bool gratis = false)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "LoL" });
            }

            var sId = long.Parse(sorID);

            if (gratis)
            {
                var sor = BDD.TBL_SORTEO.FirstOrDefault(o => o.SOR_ID == sId);
                if (!sor.SOR_GRATIS)
                {
                    msj = IngresarSorteo(sId);

                    if (msj != null)
                    {
                        ViewBag.Mensaje = msj;
                    }

                    var user = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);

                    UsuarioLogged = user;

                    ViewBag.Cantidad = user.USU_SOR_DISP;
                    ViewBag.Sorteos = N_Sorteo.Sorteos();
                    return View();
                }

                TBL_USUARIO usuario;
                try
                {
                    usuario = UsuarioLogged;
                }
                catch (Exception)
                {
                    var d = Session["User"].ToString();
                    UsuarioLogged = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == d);
                    usuario = UsuarioLogged;
                    throw;
                }

                var nub = BDD.NUB_SORTEO_USUARIO.Where(o => o.SOR_ID == sor.SOR_ID && o.USU_ID == usuario.USU_ID).ToList();
                if (nub.Count > 0)
                {
                    msj = "Ya estas participando en este sorteo gratis. Buena suerte";
                    if (msj != null)
                    {
                        ViewBag.Mensaje = msj;
                    }
                    ViewBag.Sorteos = N_Sorteo.Sorteos();
                    return View();
                }
                var nuevo = new NUB_SORTEO_USUARIO
                {
                    SOR_ID = sor.SOR_ID,
                    USU_ID = UsuarioLogged.USU_ID,
                };

                BDD.NUB_SORTEO_USUARIO.Add(nuevo);
                BDD.SaveChanges();
                msj = "Inscrito!";
                if (msj != null)
                {
                    ViewBag.Mensaje = msj;
                }
                ViewBag.Sorteos = N_Sorteo.Sorteos();
                return View();
            }

            msj = IngresarSorteo(sId);

            if (msj != null)
            {
                ViewBag.Mensaje = msj;
            }

            var usr = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);

            UsuarioLogged = usr;
            ViewBag.Cantidad = usr.USU_SOR_DISP;
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

            using (var db = new LoteriaGGEntities())
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
            using (var db = new LoteriaGGEntities())
            {
                var now = DateTime.Now.AddHours(1);
                var data = db.TBL_SORTEO.Where(o => o.SOR_FECHA_FIN.Value >= now && !o.SOR_GRATIS).ToList().Select(item => new
                {
                    Id = "#" + item.SOR_ID.ToString(),
                    FInicio = item.SOR_FECHA_INICIO?.ToString("dd'/'MM'/'yyyy hh:mm"),
                    FFin = item.SOR_FECHA_FIN?.ToString("dd'/'MM'/'yyyy hh:mm"),
                    action = "<form action=\"/Steam/Sorteo/Index\" method=\"post\" role=\"form\" >" +
                        "<input type=\"hidden\" name=\"sorID\" value=\"" + item.SOR_ID + "\">" +
                        "<a href='javascript:void()' onclick='$(this).parent().submit()' class=\"btn btn-warning\" style='font-size:15pt'><i class='fa fa-pencil-square-o' aria-hidden='true'></i></a>" +
                        "</form>",
                    Premio = item.SOR_PREMIO,
                }).ToList();

                //Segunda lista, para los sorteos gratis
                var bata = db.TBL_SORTEO.Where(o => o.SOR_FECHA_FIN.Value >= now && o.SOR_GRATIS).ToList().Select(item => new
                {
                    Id = "#" + item.SOR_ID.ToString(),
                    FInicio = item.SOR_FECHA_INICIO?.ToString("dd'/'MM'/'yyyy hh:mm"),
                    FFin = item.SOR_FECHA_FIN?.ToString("dd'/'MM'/'yyyy hh:mm"),
                    action = "<form action=\"/Steam/Sorteo/Index\" method=\"post\" role=\"form\" >" +
                        "<input type=\"hidden\" name=\"sorID\" value=\"" + item.SOR_ID + "\" />" +
                        "<input type=\"hidden\" name=\"gratis\" value=\"true\" />" +
                        "<a href='javascript:void()' onclick='$(this).parent().submit()' class=\"btn btn-warning\" style='font-size:15pt'><i class='fa fa-pencil-square-o' aria-hidden='true'></i></a>" +
                        "</form>",
                    Premio = item.SOR_PREMIO,
                }).ToList();
                foreach (var item in bata)
                {
                    data.Add(item);
                }
                return Json(new
                {
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Inscripcion(string msj = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
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