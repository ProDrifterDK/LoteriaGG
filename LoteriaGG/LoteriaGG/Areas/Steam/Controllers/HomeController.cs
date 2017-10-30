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

using LoteriaGG.Base;

namespace LoteriaGG.Areas.Steam.Controllers
{
    public class HomeController : BaseController
    {
        // GET: /Steam/Home/
        public ActionResult Index(string sum = "", string msg2 = "", string mensaje ="")
        {
            if (Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }
            if (sum != "")
            {
                ViewBag.Summ = sum;
            }
            if (msg2 != "")
            {
                ViewBag.Mensaje2 = msg2;
            }
            if(mensaje != "")
            {
                ViewBag.Mensaje = mensaje;
            }

            if (UsuarioLogged.USU_EMAIL == null && UsuarioLogged.USU_FACEBOOK_ID != null)
            {
                ViewBag.NecesitaMail = 1;
            }
            TBL_HOME txtHome = BDD.TBL_HOME.FirstOrDefault(o => o.HM_ID == 2);
            if (txtHome == null)
            {
                txtHome = new TBL_HOME { HM_TXT_GANADORES = "No hay ganadores todavía" };
                BDD.TBL_HOME.Add(txtHome);
                BDD.SaveChanges();
            }
            ViewBag.Ganadores = txtHome.HM_TXT_GANADORES;
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
                    UsuarioLogged = user;
                    Session["LogedIn"] = "True";
                    Session["User"] = user.USU_ACCOUNT;
                    Session["UserN"] = user.USU_NOMBRE;
                    if (user.USU_VERIFICADO == null || user.USU_VERIFICADO == false)
                    {
                        ViewBag.Mensaje = "Recuerda revisar tu mail para verificar tu cuena.";
                    }
                    if (user.USU_STEAM_NICK == "" || user.USU_STEAM_NICK == null)
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
                    ViewBag.Mensaje = "Cuenta creada correctamente.";
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
            }
            else if (Session["LogedIn"].ToString() != "")
            {
                return RedirectToAction("Index", "Home", new { area = "LoL" });
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
                }
                else
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
            TBL_USUARIO usu;
            if (!mail.Contains("@"))
                usu = FBRegSinMail(mail, usr);
            else
                usu = FBLog(mail);
            if (usu == null)
            {
                usu = FBReg(mail, usr);
            }
            UsuarioLogged = usu;
            Session["LogedIn"] = "True";
            Session["User"] = usu.USU_ACCOUNT;
            Session["UserN"] = usu.USU_NOMBRE;
            if (usu.USU_STEAM_NICK == "" || usu.USU_STEAM_NICK == null)
                return RedirectToAction("Index", new { sum = "NoTiene" });
            return RedirectToAction("Index");

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
                    usu.USU_PAGADO = false;
                    usu.USU_USO_REFER = false;
                    usu.USU_REFER_CODIGO = "ref-" + usu.USU_ACCOUNT;

                    dc.TBL_USUARIO.Add(usu);

                    dc.SaveChanges();
                    return dc.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == mail);
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
        public ActionResult Summoner(string summoner)
        {
            using (var db = new LOTERIA_GGEntities())
            {
                var usu = Session["User"].ToString();
                var usr = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu);
                usr.USU_STEAM_NICK = summoner;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}