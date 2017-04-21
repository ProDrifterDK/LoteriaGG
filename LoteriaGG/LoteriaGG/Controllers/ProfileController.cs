using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos.SqlData;
    using System.Text.RegularExpressions;
using Negocio;
using System.Net.Mail;

namespace LoteriaGG.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            return View(us);
        }

        public ActionResult CambiarContrasena()
        {
            return View();
        }

        public ActionResult ObtenerSorteo(string msj = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            if (msj != null)
            {
                ViewBag.Mensaje = msj;
            }

            return View();
        }

        [HttpPost]
        public ActionResult CambiarContrasena(string nContrasena, string nContrasenaR, string aContrasena)
        {
            //retornar vista diferente dependiendo de si es correcto o no *** IMPORTANTE!
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }
            var respuesta = Class1.CambiarContrasena(Session["User"].ToString(), nContrasena, nContrasenaR, aContrasena);

            ViewBag.cambio = respuesta;

            return View();
        }

        public ActionResult EditarPerfil()
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            TBL_USUARIO us = Class1.ObtenerCuenta(Session["User"].ToString());

            return View(us);
        }

        [HttpPost]
        public ActionResult EditarPerfil(string nombre, string apellido, string nombreInvocador)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
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
                return RedirectToAction("Index", "Home", new { });
            }

            if (codigo == "RakanXayah")
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    var usr = Session["User"].ToString();
                    var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usr);

                    if (usrD.USU_SORTEO_ESPECIAL == false)
                    {
                        return RedirectToAction("ObtenerSorteo", new { msj = "Ya usaste este codigo." });
                    }

                    usrD.USU_SOR_DISP++;
                    db.SaveChanges();
                }
            }
            Guid Codigo;
            try
            {
                Codigo = Guid.Parse(codigo);
            }
            catch
            {
                return RedirectToAction("ObtenerSorteo", new { msj = "El codigo ingresado es incorrecto." });
            }
            using (var db = new LOTERIA_GGEntities())
            {
                var sg = db.TBL_SORTEO_GRATIS.FirstOrDefault(o => o.SG_CODIGO == Codigo && o.SG_VALIDO == true); 
                if(sg == null)
                {
                    return RedirectToAction("ObtenerSorteo", new { msj = "El codigo ingresado es incorrecto." });
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
                    return RedirectToAction("ObtenerSorteo", new { msj = "Se ha cargado un sorteo a tu cuenta." });
                }
            }
        }

        public ActionResult EditarMail(int? rp = null)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
            }

            ViewBag.cambio = rp;

            return View();
        }
        [HttpPost]
        public ActionResult EditarMail(string newMail)
        {
            if (Session["LogedIn"] == null)
            {
                return RedirectToAction("Index", "Home", new { });
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
                "<p><a href=" + "\"http://prueba.loteriagg.com/Profile/CambioEmail?us=" + username + "&nm=\"" + newM + "\"\"" + ">Preciona aquí para verificar</a></p>"
                + "<p>Si tienes probelmas con el link copia y pega el siguiente link http://prueba.loteriagg.com/Profile/CambioEmail?us=" + username + "&nm=" + newM + "</p>" +
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
                    return RedirectToAction("Index", "Home", new { });
                }

                user.USU_CAMBIO_EMAIL = false;
                user.USU_EMAIL = nm;
                db.SaveChanges();
            }
            return RedirectToAction("EditarMail", new { rp = 0});
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
    }
}