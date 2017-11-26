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
using System.Web.Routing;
using System.Data.Entity.Validation;

namespace LoteriaGG.Base
{
    public class BaseController : Controller
    {
        protected LoteriaGGEntities BDD;
        protected BaseController()
        {
            BDD = new LoteriaGGEntities();
        }

        protected TBL_USUARIO UsuarioLogged
        {
            get { return Session["UsuarioLogged"] == null ? new TBL_USUARIO() : (TBL_USUARIO)Session["UsuarioLogged"]; }
            set { Session["UsuarioLogged"] = value; }
        }

        protected string Register(string usuario, string pass, string email, string confirm, string nombre, string apellido, string nombreDeInvocador, bool terminos)
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
                    if (BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usuario) != null)
                    {
                        throw new Exception("Usuario ya registrado");
                    }
                    else if (BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == email) != null)
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
                    usu.USU_VERIFICADO = true;
                    usu.USU_PAGADO = false;
                    usu.USU_USO_REFER = false;
                    usu.USU_REFER_CODIGO = "ref-" + usuario;

                    BDD.TBL_USUARIO.Add(usu);
                    BDD.SaveChanges();
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
                using (var db = new LoteriaGGEntities())
                {
                    var usr = db.TBL_USUARIO.Where(o => o.USU_EMAIL == mail).FirstOrDefault();
                    if (usr == null)
                    {
                        return RedirectToAction("Verification", new { us = usr, verif = usr, msj = "Email no concide." });
                    }
                    SendEmailConfirmation(mail, usr.USU_ACCOUNT, usr.USU_CODIGO_VERIFICAION.ToString(), usr.USU_NOMBRE);
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Verification", "Home", new { area = "LoL" });
        }

        protected TBL_USUARIO FBRegSinMail(string id, string usr)
        {
            if (BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_FACEBOOK_ID == id) != null)
            {
                return BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_FACEBOOK_ID == id);
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
            usu.USU_FACEBOOK_ID = id;
            usu.USU_SUMMONER = "";
            usu.USU_CODIGO_VERIFICAION = Guid.Empty;
            usu.USU_VERIFICADO = true;
            usu.USU_PAGADO = false;
            usu.USU_USO_REFER = false;
            usu.USU_REFER_CODIGO = "ref-" + usu.USU_ACCOUNT;

            BDD.TBL_USUARIO.Add(usu);

            BDD.SaveChanges();
            return BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_FACEBOOK_ID == id);
        }

        [HttpPost]
        public ActionResult SetMail(string mail)
        {
            if(mail == null || mail == "")
                return RedirectToAction("Index", "Home", new { mensaje = "mail no puede estar vacío" });

            if (!mail.Contains("@"))
                return RedirectToAction("Index", "Home", new { mensaje = "Email Invalido" });

            if (UsuarioLogged == null)
            {
                return RedirectToAction("Index");
            }
            var us = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == mail);
            if (us != null)
            {
                return RedirectToAction("Index", "Home", new { mensaje = "Email ya registrado" });
            }
            var user = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);
            user.USU_EMAIL = mail;
            BDD.TBL_USUARIO.Attach(user);
            BDD.Entry(user).State = System.Data.Entity.EntityState.Modified;
            BDD.SaveChanges();

            UsuarioLogged = user;           // Redefinimos el usuario logueado para que contenga el mail.

            return RedirectToAction("Index", "Home", new { msg2 = "Mail Actualizado"});
        }

        protected TBL_USUARIO FBReg(string mail, string usr)
        {
            try
            {
                using (var dc = new LoteriaGGEntities())
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
        protected TBL_USUARIO FBLog(string mail)
        {
            TBL_USUARIO ret = null;
            try
            {
                using (var dc = new LoteriaGGEntities())
                {
                    ret = dc.TBL_USUARIO.FirstOrDefault(o => o.USU_EMAIL == mail);
                }
            }
            catch (Exception ex)
            {

            }

            return ret;
        }
    }
}