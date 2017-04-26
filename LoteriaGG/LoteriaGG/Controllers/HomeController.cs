using System;
using System.Data;
using System.Data.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;
using Datos.SqlData;
using System.Net.Mail;
using System.Net;
using BotDetect;
using BotDetect.Web.Mvc;
using BotDetect.Web.UI;
using System.Web.Routing;

namespace LoteriaGG.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password, string email = "", string confirmPassword = "", string type = "login", string Nombre = "", string Apellido = "", string NombreDeInvocador = "", bool terminos = false)
        {
            if (type == "login")
            {
                var user = Log(username, password);
                if (user != null)
                {
                    Session["LogedIn"] = "True";
                    Session["User"] = user.USU_ACCOUNT;
                    if(user.USU_VERIFICADO == null || user.USU_VERIFICADO == false)
                    {
                        ViewBag.Mensaje = "Recuerda revisar tu mail para verificar tu cuena.";
                    }
                }
                else
                {
                    ViewBag.Mensaje = "Ceunta o contraseña ingresados son incorrectos.";
                }
            }
            else
            {
                var reg = Register(username, password, email, confirmPassword, Nombre, Apellido, NombreDeInvocador, terminos);
                if (reg == "success")
                {
                    ViewBag.Mensaje = "Cuenta creada correctamente, te enviaremos un mail de verificacion, pero puedes usar tu cuenta sin problemas. Buena suerte";
                }
                else
                {
                    ViewBag.Mensaje = reg;
                }
            }
            return View();
        }


        private TBL_USUARIO Log(string usu, string pass)
        {
            TBL_USUARIO ret = null;
            try
            {
                using (var dc = new LOTERIA_GGEntities())
                {
                    ret = dc.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu && o.USU_PASSWORD == pass);
                }
            }
            catch (Exception ex)
            {

            }

            return ret;
        }

        private string Register(string usuario, string pass, string email, string confirm, string nombre, string apellido, string nombreDeInvocador, bool terminos)
        {

            if (!terminos)
            {
                return "Debe aceptar los terminos de usuario";
            }
            if (confirm != pass)
            {
                return "Contraseñas no coinciden";
            }
            try
            {
                using (var dc = new LOTERIA_GGEntities())
                {
                    if (dc.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usuario) != null)
                    {
                        throw new Exception("Usuario ya registrado");
                    }
                    else if (dc.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == email) != null)
                    {
                        throw new Exception("Email ya registrado");
                    }
                    var activationCode = Guid.NewGuid();

                    var usu = new TBL_USUARIO();
                    usu.USU_ACCOUNT = usuario;
                    usu.USU_PASSWORD = pass;
                    usu.USU_NOMBRE = nombre;
                    usu.USU_APELLIDO = apellido;
                    usu.USU_EMAIL = email;
                    usu.USU_SUMMONER = nombreDeInvocador;
                    usu.USU_CODIGO_VERIFICAION = activationCode;
                    usu.USU_VERIFICADO = false;

                    dc.TBL_USUARIO.Add(usu);
                    try
                    {
                        SendEmailConfirmation(email, usuario, activationCode.ToString(), nombre);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message + ex.InnerException ?? "";
                    }
                    dc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message + ex.InnerException ?? "";
            }
            return "success";
        }

        private void SendEmailConfirmation(string to, string username, string confirmationToken, string name)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(new MailAddress(to));
            mm.From = new MailAddress("noreply@loteriagg.com");
            mm.Body = "<h3>Bienvenido a LoteriaGG, " + name + " Gracias por registrarte. </h3> <p>Tus datos son:</p> <p>Nombre de Usuario: " + username + "</p> <p>Tu direccion de Email: " + to + "</p>" +
                "<p>Para verificar el email debes presionar el siguiente link</p><a href=" + "\"http://loteriagg.com/Home/Verification?us=" + username + "&verif=" + confirmationToken + "\"" + ">Preciona aquí para verificar</a>"
                + "<p>Si tienes probelmas con el link copia y pega el siguiente link http://loteriagg.com/Home/Verification?us=" + username + "&verif=" + confirmationToken + "</p>";
            mm.IsBodyHtml = true;
            mm.Subject = "Verification";
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mm);
        }

        [HttpPost]
        public ActionResult Reenviar(string mail)
        {
            try
            {
                using (var db = new LOTERIA_GGEntities())
                {
                    var usr = db.TBL_USUARIO.Where(o => o.USU_EMAIL == mail).FirstOrDefault();
                    if (usr == null)
                    {
                        return RedirectToAction("Verification", new { us = usr, verif = usr, msj = "Email no concide." });
                    }
                    SendEmailConfirmation(mail, usr.USU_ACCOUNT, usr.USU_CODIGO_VERIFICAION.ToString(), usr.USU_NOMBRE);
                }
            } catch (Exception ex)
            {

            }
            return RedirectToAction("Verification", "Home");
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session["LogedIn"] = "";
            return RedirectToAction("Index");
        }

        public ActionResult Verification(string us, string verif, string msj)
        {
            if (Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            } else if (Session["LogedIn"].ToString() != "")
            {
                return RedirectToAction("Index", "Home", new { });
            }
            if (us == "" && verif == "")
                ViewBag.ret = null;
            else
            {
                if (us != null && verif != null)
                {
                    Session["LogedIn"] = "True";
                    Session["User"] = Class1.Verificar(us, verif).USU_ACCOUNT;
                    ViewBag.ret = true;
                } else
                    ViewBag.ret = null;
            }
            ViewBag.Mensaje = msj;
            return View();
        }

        public ActionResult Contactanos()
        {
            return View();
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });
        }
    }
}