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

namespace LoteriaGG.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if(Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password, string email = "", string confirmPassword = "", string type = "login", string Nombre = "", string Apellido ="", string NombreDeInvocador = "")
        {
            if(type == "login")
            {
                var user = Log(username, password);
                if (user != null)
                {
                    Session["LogedIn"] = "True";
                    Session["User"] = user.USU_ACCOUNT;
                }
            }
            else
            {
                var reg = Register(username, password, email, confirmPassword, Nombre, Apellido, NombreDeInvocador);
                if(reg == "success")
                {
                    return RedirectToAction("Verification", "Home", new { us="", verif="" });
                }
                else
                {
                    ViewBag.Mensaje = reg;
                }
            }
            return View();
        }

        TBL_USUARIO Log(string user, string pass)
        {
            return Class1.LogIn(user, pass);
        }

        private string Register(string usuario, string pass, string email,string confirm, string nombre, string apellido, string nombreDeInvocador)
        {
            if(confirm != pass)
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
                    SendEmailConfirmation(email, usuario, activationCode.ToString(), nombre);
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
            mm.From = new MailAddress("loteriagg@noreply.com");
            mm.Body = "<h3>Bienvenido a LoteriaGG, " + name + " Gracias por registrarte. </h3> <p>Tus datos son:</p> <p>Nombre de Usuario: " + username + "</p> <p>Tu direccion de Email: " + to + "</p>" +
                "<p>Para verificar el email debes presionar el siguiente link</p><a href=" + "\"http://prueba.loteriagg.com/Home/Verification?us=" + username + "&verif=" + confirmationToken + "\"" + ">Preciona aquí para verificar</a>"
                + "<p>Si tienes probelmas con el link copia y pega el siguiente link http://prueba.loteriagg.com/Home/Verification?us=" + username + "&verif=" + confirmationToken + "</p>";
            mm.IsBodyHtml = true;
            mm.Subject = "Verification";
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mm);
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session["LogedIn"] = "";
            return RedirectToAction("Index");
        }

        public ActionResult Verification(string us, string verif)
        {
            if(Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }else if(Session["LogedIn"].ToString() != "")
            {
                return RedirectToAction("Index", "Home", new { });
            }
            if (us == "" && verif == "")
                ViewBag.ret = null;
            else
            {
                if(us != null && verif != null)
                {
                    Session["LogedIn"] = "True";
                    Session["User"] = Class1.Verificar(us, verif).USU_ACCOUNT;
                    ViewBag.ret = true;
                }else
                    ViewBag.ret = null;
            }
            return View();
        }
    }
}