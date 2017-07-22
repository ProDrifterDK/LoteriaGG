using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos.SqlData;
    using System.Text.RegularExpressions;
using Negocio;
using System.Net.Mail;
using System.Collections;

namespace LoteriaGG.Areas.Steam.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /Steam/Profile/
        public ActionResult Index()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new {area ="Steam"});
            }

            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            return View(us);
        }

        public ActionResult CambiarContrasena()
        {
            return View();
        }

        public ActionResult ObtenerGGCoins(string msj = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { Area = "Steam"});
            }

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
            return View();
        }

        [HttpPost]
        public ActionResult CambiarContrasena(string nContrasena, string nContrasenaR, string aContrasena)
        {
            //retornar vista diferente dependiendo de si es correcto o no *** IMPORTANTE!
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }
            var respuesta = Class1.CambiarContrasena(Session["User"].ToString(), nContrasena, nContrasenaR, aContrasena);

            ViewBag.cambio = respuesta;

            return View();
        }

        public ActionResult EditarPerfil()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }

            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            return View(us);
        }

        [HttpPost]
        public ActionResult EditarPerfil(string nombre, string apellido, string nombreInvocador)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area="Steam" });
            }
            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            ViewBag.tr = Class1.EditarPerfil(Session["User"].ToString(), nombre, apellido, nombreInvocador);

            return View(us);
        }

        [HttpPost]
        public ActionResult SorteoGratis(string codigo)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }

            if (codigo == "GIVEMERUST")
            {
                using (var db = new LOTERIA_GGEntities())
                {
                    var usr = Session["User"].ToString();
                    var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usr);

                    if (usrD.USU_SORTEO_ESPECIAL == true)
                    {
                        return RedirectToAction("ObtenerGGCoins", new { msj = "Ya usaste este codigo." });
                    }
                    var sorteo = db.TBL_SORTEO.FirstOrDefault(o => o.SOR_ID == 11);

                    var nubSor = new NUB_SORTEO_USUARIO { SOR_ID = sorteo.SOR_ID, USU_ID = usrD.USU_ID };

                    db.NUB_SORTEO_USUARIO.Add(nubSor);
                    usrD.USU_SORTEO_ESPECIAL = true;
                    db.SaveChanges();

                    return RedirectToAction("ObtenerGGCoins", new { msj = "Ahora estas inscrito en el sorteo para ganar RUST!." });
                }
            }
            Guid Codigo;
            try
            {
                Codigo = Guid.Parse(codigo);
            }
            catch
            {
                return RedirectToAction("ObtenerGGCoins", new { area = "Steam",  msj = "El codigo ingresado es incorrecto." });
            }
            using (var db = new LOTERIA_GGEntities())
            {
                var sg = db.TBL_SORTEO_GRATIS.FirstOrDefault(o => o.SG_CODIGO == Codigo && o.SG_VALIDO == true); 
                if(sg == null)
                {
                    return RedirectToAction("ObtenerGGCoins", new { area = "Steam", msj = "El codigo ingresado es incorrecto." });
                }
                else
                {
                    var usr = Session["User"].ToString();
                    var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usr);
                    if(usrD.USU_SOR_DISP == null)
                    {
                        usrD.USU_SOR_DISP = 0;
                    }
                    usrD.USU_SOR_DISP++;    
                    sg.SG_VALIDO = false;
                    sg.USU_ID = usrD.USU_ID;
                    db.SaveChanges();
                    return RedirectToAction("ObtenerGGCoins", new { area = "Steam", msj = "Se ha cargado un GGCoin a tu cuenta." });
                }
            }
        }

        public ActionResult EditarMail(int? rp = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }

            ViewBag.cambio = rp;

            return View();
        }
        [HttpPost]
        public ActionResult EditarMail(string newMail)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }
            if (!IsEmailValid(newMail))
            {
                ViewBag.Mensaje = "No es un mail válido";
                return View();
            }
            var us = Session["User"].ToString();

            using(var db = new LOTERIA_GGEntities())
            {
                var user = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == us);
                if(db.TBL_USUARIO.Where(o => o.USU_EMAIL == newMail).ToList().Count > 0)
                {
                    ViewBag.Mensaje = "El mail ya esta en uso";
                    return View();
                }

                user.USU_CAMBIO_EMAIL = true;

                SendEmailConfirmation(user.USU_EMAIL, us, user.USU_NOMBRE, newMail);
                db.SaveChanges();
            }
            ViewBag.cambio = 1;
            return View();
        }
        private void SendEmailConfirmation(string to, string username, string name, string newM)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(new MailAddress(to));
            mm.From = new MailAddress("noreply@loteriagg.com");
            mm.Body = "<h3>Hola " + name + ". </h3> <p>Si has solicitado un cambio de dirección de correo electrónico, haz clic o copia y pega el siguiente enlace. </p>" +
                "<p><a href=" + "\"http://loteriagg.com/Profile/CambioEmail?us=" + username + "&nm=\"" + newM + "\"\"" + ">Preciona aquí para verificar</a></p>"
                + "<p>Si tienes probelmas con el link copia y pega el siguiente link http://loteriagg.com/Profile/CambioEmail?us=" + username + "&nm=" + newM + "</p>" +
                "<p>Si no has solicitado ningún cambio, cambia tu contraseña y avisa al soporte de LoteriaGG al E-mail soporte@LoteriaGG.com</p>. <p>Se despide, el equipo de LoteriaGG.</p>";
            mm.IsBodyHtml = true;
            mm.Subject = "Verification";
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mm);
        }

        public ActionResult CambioEmail(string us, string nm)
        {
            using (var db = new LOTERIA_GGEntities())
            {
                var user = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == us && o.USU_CAMBIO_EMAIL == true);

                if (user == null)
                {
                    return RedirectToAction("Index", "Home", new { area = "Steam" });
                }

                user.USU_CAMBIO_EMAIL = false;
                user.USU_EMAIL = nm;
                db.SaveChanges();
            }
            return RedirectToAction("EditarMail", new { area = "Steam", rp = 0});
        }

        public static Boolean IsEmailValid(string EmailAddr)
        {

            if (EmailAddr != null || EmailAddr != "")
            {
                Regex n = new Regex("(?<user>[^@]+)@(?<host>.+)");
                Match v = n.Match(EmailAddr);

                if (!v.Success || EmailAddr.Length != v.Length)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public ActionResult SorteosInscritos()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Steam" });
            }

            using(var db = new LOTERIA_GGEntities())
            {
                var usr = Session["User"].ToString();
                var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usr);

                var nUS = db.NUB_SORTEO_USUARIO.Where(o => o.USU_ID == usrD.USU_ID);

                if(nUS == null)
                {
                    ViewBag.tieneSorteos = true;
                }
            }

            return View();
        }

        public JsonResult DatosSorteosInscritos()
        {
            var rtn = new ArrayList();

            using (var db = new LOTERIA_GGEntities())
            {
                var sUsr = Session["User"].ToString();
                var usr = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == sUsr);
                var nSU = db.NUB_SORTEO_USUARIO.Where(o => o.USU_ID == usr.USU_ID).ToList();
                if(nSU != null)
                foreach (var item in nSU)
                {
                    rtn.Add(new
                    {
                        Id = item.SOR_ID,
                        FFinicio = item.TBL_SORTEO.SOR_FECHA_INICIO?.ToString("dd'/'MM'/'yyyy hh:mm"),
                        FFin = item.TBL_SORTEO.SOR_FECHA_FIN?.ToString("dd'/'MM'/'yyyy hh:mm"),
                        Premio = item.TBL_SORTEO.SOR_PREMIO,
                    });
                }
            }
            return Json(new
            {
                data = rtn
            }, JsonRequestBehavior.AllowGet);
        }
    }
}