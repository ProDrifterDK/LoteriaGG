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
using System.Data.Entity.Validation;

namespace LoteriaGG.Areas.LoL.Controllers
{
    public class HomeController : Controller
    {
        // GET: /LoL/Home/
        public ActionResult Index(string sum = "", string msg2 = "")
        {
            if (Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }
            if(sum != "")
            {
                ViewBag.Summ = sum;
            }
            if (msg2 != "")
            {
                ViewBag.Mensaje2 = msg2;
            }
            using (var db = new LOTERIA_GGEntities())
            {
                TBL_HOME txtHome = db.TBL_HOME.FirstOrDefault();
                if(txtHome == null)
                {
                    txtHome = new TBL_HOME { HM_TXT_GANADORES = "No hay ganadores todavía" };
                    db.TBL_HOME.Add(txtHome);
                    db.SaveChanges();
                }
                ViewBag.Ganadores = txtHome.HM_TXT_GANADORES;
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
                    Session["UserN"] = user.USU_NOMBRE;
                    using (var db = new LOTERIA_GGEntities())
                    {
                        var usu = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user.USU_ACCOUNT);
                        if (usu.USU_DAILY_REWARD == null || usu.USU_DAILY_REWARD.Value.Day != DateTime.Now.Day)
                        {
                            if (usu.USU_DAILY == null)
                            {
                                usu.USU_DAILY = 0;
                            }
                            usu.USU_DAILY++;
                            ViewBag.Mensaje2 = "Regalo Diario! por cada tres días que te conectes ganas un GGCoin. Llevas " + usu.USU_DAILY +" de 3.";
                            if (usu.USU_DAILY == 3)
                            {
                                ViewBag.Mensaje2 = "Haz ganado una GGCoin por conectarte tres días!!! Sigue así";
                                usu.USU_SOR_DISP++;
                                usu.USU_DAILY = 0;
                            }

                            db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu.USU_ACCOUNT).USU_DAILY_REWARD = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    if (user.USU_VERIFICADO == null || user.USU_VERIFICADO == false)
                    {
                        ViewBag.Mensaje = "Recuerda revisar tu mail para verificar tu cuena.";
                    }
                    if(user.USU_SUMMONER == "" || user.USU_SUMMONER == null)    
                    {
                        ViewBag.Summ = "NoTiene";
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
                "<p>Para verificar tu email debes seguir el siguiente enlace:</p><a href=" + "\"http://loteriagg.com/LoL/Home/Verification?us=" + username + "&verif=" + confirmationToken + "\"" + ">Verificar</a>"
                + "<p>Si tienes probelmas con el link copia y pega el siguiente link http://loteriagg.com/LoL/Home/Verification?us=" + username + "&verif=" + confirmationToken + "</p>";
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
            return RedirectToAction("Verification", "Home",new { area = "LoL" });
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
                return RedirectToAction("Index", "Home", new { area = "LoL"});
            }
            if (us == "" && verif == "")
                ViewBag.ret = null;
            else
            {
                if (us != null && verif != null)
                {
                    Session["LogedIn"] = "True";
                    var usu = Class1.Verificar(us, verif);
                    Session["User"] = usu.USU_ACCOUNT;
                    Session["UserN"] = usu.USU_NOMBRE;
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

        public ActionResult FBLogin(string mail, string usr)
        {
            var usu = FBLog(mail);
            if (usu == null)
            {
                usu = FBReg(mail, usr);
            }
            Session["LogedIn"] = "True";
            Session["User"] = usu.USU_ACCOUNT;
            Session["UserN"] = usu.USU_NOMBRE;
            string msg2 = "";
            using (var db = new LOTERIA_GGEntities())
            {
                var user = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu.USU_ACCOUNT);
                if (user.USU_DAILY_REWARD == null || user.USU_DAILY_REWARD.Value.Day != DateTime.Now.Day)
                {
                    if (user.USU_DAILY == null)
                    {
                        user.USU_DAILY = 0;
                    }
                    user.USU_DAILY++;
                    msg2 = "Regalo Diario! por cada tres días que te conectes ganas un GGCoin. Llevas " + usu.USU_DAILY +" de 3.";
                    if (user.USU_DAILY == 3)
                    {
                        msg2 = "Haz ganado una GGCoin por conectarte tres días!!! Sigue así";
                        user.USU_SOR_DISP++;
                        user.USU_DAILY = 0;
                    }

                    user.USU_DAILY_REWARD = DateTime.Now;
                    db.SaveChanges();
                }
            }
            if (usu.USU_STEAM_NICK == "" || usu.USU_STEAM_NICK == null)
                return RedirectToAction("Index", new { sum = "NoTiene" });
            return RedirectToAction("Index", new { msg2 = msg2 });

        }

        private TBL_USUARIO FBReg(string mail, string usr)
        {
            try
            {
                using (var dc = new LOTERIA_GGEntities())
                {
                    if (dc.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == mail) != null)
                    {
                        throw new Exception("Mail ya registrado");
                    }
                    var activationCode = Guid.NewGuid();

                    var usu = new TBL_USUARIO();
                    string[] s = usr.Split(' ');
                    Random rand = new Random();
                    string ss = usr + rand.Next().ToString();
                    usu.USU_ACCOUNT = ss.Substring(0, 20);
                    usu.USU_PASSWORD = "FBLog" + rand.Next().ToString();
                    usu.USU_NOMBRE = s[0];
                    if (s.Length == 3 || s.Length == 2)
                        usu.USU_APELLIDO = s[1];
                    else if (s.Length > 3)
                        usu.USU_APELLIDO = s[2];
                    usu.USU_EMAIL = mail;
                    usu.USU_SUMMONER = "";
                    usu.USU_CODIGO_VERIFICAION = Guid.Empty;
                    usu.USU_VERIFICADO = true;

                    dc.TBL_USUARIO.Add(usu);

                    dc.SaveChanges();
                    return dc.TBL_USUARIO.FirstOrDefault(o=> o.USU_EMAIL == mail);
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }
        private TBL_USUARIO FBLog(string mail)
        {
            TBL_USUARIO ret = null;
            try
            {
                using (var dc = new LOTERIA_GGEntities())
                {
                    ret = dc.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == mail);
                }
            }
            catch (Exception ex)
            {

            }

            return ret;
        }

        [HttpPost]
        public ActionResult Summoner (string summoner)
        {
            using(var db = new LOTERIA_GGEntities())
            {
                var usu = Session["User"].ToString();
                var usr = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu);
                usr.USU_SUMMONER = summoner;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}