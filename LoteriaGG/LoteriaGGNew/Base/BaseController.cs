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
using System.Web.Script.Serialization;
using System.Text;
using System.IO;

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
            get { return Session["UsuarioLogged"] == null ? null : (TBL_USUARIO)Session["UsuarioLogged"]; }
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
            if (mail == null || mail == "")
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

            return RedirectToAction("Index", "Home", new { msg2 = "Mail Actualizado" });
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

        [HttpPost]
        public JsonResult JsonGetListaSorteos()
        {
            var datos = BDD.TBL_SORTEO.Where(o => o.SOR_FECHA_FIN > DateTime.Now && o.SOR_FECHA_INICIO < DateTime.Now).ToList().Select(o => new
            {
                id = o.SOR_ID,
                fechaF = o.SOR_FECHA_FIN?.ToString("dd/MM/yy hh:mm"),
                fechaI = o.SOR_FECHA_INICIO?.ToString("dd/MM/yy hh:mm"),
                Premio = o.SOR_PREMIO,
            }).ToList();

            return Json(new { data = datos }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IngresarSorteo(int sorId)
        {
            if (UsuarioLogged == null)
            {
                return Json(new { data = "Sin Usuario" }, JsonRequestBehavior.AllowGet);
            }
            if (UsuarioLogged.USU_SOR_DISP < 1)
                return Json(new { data = "No tienes suficientes GGCoins" });

            var sorteo = BDD.TBL_SORTEO.FirstOrDefault(o => o.SOR_ID == sorId);
            var nub = new NUB_SORTEO_USUARIO
            {
                SOR_ID = sorId,
                USU_ID = UsuarioLogged.USU_ID,
            };

            BDD.NUB_SORTEO_USUARIO.Add(nub);
            BDD.Entry(nub).State = System.Data.Entity.EntityState.Added;

            var usuario = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);
            usuario.USU_SOR_DISP = usuario.USU_SOR_DISP - 1;

            BDD.TBL_USUARIO.Attach(usuario);
            BDD.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

            BDD.SaveChanges();

            UsuarioLogged = usuario;

            return Json(new { data = new { mensaje = "Exito", GGCoins = UsuarioLogged.USU_SOR_DISP } }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EnviarNotificacion(string codigo)
        {
            try
            {
                using (var db = new LoteriaGGEntities())
                {
                    var cod = new CODIGOS_PAGO_RUT
                    {
                        PARU_CODIGO = codigo,
                        USU_ID = UsuarioLogged.USU_ID
                    };

                    db.CODIGOS_PAGO_RUT.Add(cod);
                    db.SaveChanges();
                }
                var applicationID = "AIzaSyDm-KAY0b_0wwB63TW3R-LxYAsHf3V3inU";
                var senderId = "1049140455189";
                string deviceId = "/topics/codigoPR";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = codigo,
                        title = "Nuevo Codigo",
                        icon = "myicon",
                        sound = "mySound",
                    },
                    priority = "high"

                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                Console.Write(sResponseFromServer);
                            }
                        }
                    }
                }
                return Json(new { Exito = "exito" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { Exito = "no" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SorteoGratis(string codigo)
        {
            if (UsuarioLogged == null)
            {
                return RedirectToAction("Index", "Home", new { area = "LoL" });
            }
            Guid Codigo;
            /*Refer*/
            if (codigo.Contains("ref-"))
            {
                try
                {
                    using (var db = new LoteriaGGEntities())
                    {
                        var userRef = db.TBL_USUARIO.FirstOrDefault(o => o.USU_REFER_CODIGO == codigo);
                        var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);
                        if (usrD.USU_USO_REFER)
                            return Json(new { exito = "false", mensaje = "Ya usuaste un codigo de referencia." }, JsonRequestBehavior.AllowGet);
                        if (usrD == userRef)
                            return Json(new { exito = "false", mensaje = "No puedes referirte a ti mismo." }, JsonRequestBehavior.AllowGet);

                        if (usrD.USU_SOR_DISP == null)
                        {
                            usrD.USU_SOR_DISP = 0;
                        }
                        usrD.USU_SOR_DISP++;
                        usrD.USU_REFERENTE = userRef.USU_ID;
                        usrD.USU_USO_REFER = true;
                        db.SaveChanges();
                        UsuarioLogged = usrD;
                        return Json(new { exito = "true", mensaje = "Se ha cargado un GGCoin a tu cuenta.", GGCoins = usrD.USU_SOR_DISP }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {

                }
            }

            /*Normal*/
            try
            {
                Codigo = Guid.Parse(codigo);
            }
            catch
            {
                return Json(new { exito = "false", mensaje = "El codigo ingresado es incorrecto." }, JsonRequestBehavior.AllowGet);
            }
            using (var db = new LoteriaGGEntities())
            {
                var sg = db.TBL_SORTEO_GRATIS.FirstOrDefault(o => o.SG_CODIGO == Codigo && o.SG_VALIDO == true);
                if (sg == null)
                {
                    return Json(new { exito = "false", mensaje = "El codigo ingresado es incorrecto." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var usrD = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ID == UsuarioLogged.USU_ID);
                    if (usrD.USU_SOR_DISP == null)
                    {
                        usrD.USU_SOR_DISP = 0;
                    }
                    usrD.USU_SOR_DISP++;
                    sg.SG_VALIDO = false;
                    sg.USU_ID = usrD.USU_ID;
                    db.SaveChanges();
                    UsuarioLogged = usrD;
                    return Json(new { exito = "true", mensaje = "Se ha cargado un GGCoin a tu cuenta.", GGCoins = usrD.USU_SOR_DISP }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Registro(string argUsuario, string email, string nombre, string apellido, string contrasena, string contrasena2, string summoner = null, string steamNick = null)
        {
            if(argUsuario == null || email == null || contrasena == null || nombre == null)
            {
                return Json(new { exito = "false", mensaje = "Debese llenar todos los campos obligatorios." });
            }
            if (contrasena2 != contrasena)
            {
                return Json(new { exito = "false", mensaje = "Las contraseñas no coinciden." });
            }
            var existe = BDD.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == argUsuario || o.USU_EMAIL.ToLower() == email.ToLower());

            if(existe != null && existe.USU_ACCOUNT == argUsuario)
            {
                return Json(new { exito = "false", mensaje = "El usuario ya está registrado." });
            }
            if (existe != null && existe.USU_EMAIL == email)
            {
                return Json(new { exito = "false", mensaje = "El email ya está registrado." });
            }

            var account = new TBL_USUARIO
            {
                USU_ACCOUNT = argUsuario,
                USU_EMAIL = email,
                USU_NOMBRE = nombre,
                USU_APELLIDO = apellido,
                USU_PASSWORD = contrasena,
                USU_SOR_DISP = 0,
                USU_REFER_CODIGO = "ref-" + argUsuario,
            };

            BDD.TBL_USUARIO.Add(account);
            BDD.Entry(account).State = System.Data.Entity.EntityState.Added;

            BDD.SaveChanges();

            UsuarioLogged = account;

            return Json(new { exito = "true", mensaje = "" });
        }

        [HttpPost]
        public JsonResult EnviarMensajeContactanos(string mensaje)
        {
            var msj = new TBL_CONTACTO
            {
                USU_ID = UsuarioLogged.USU_ID,
                CONT_TEXTO = mensaje,
            };

            BDD.TBL_CONTACTO.Add(msj);
            BDD.Entry(msj).State = System.Data.Entity.EntityState.Added;

            try
            {
                BDD.SaveChanges();
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
                return Json(new { exito = false, mensaje = "Ha habido un problema, contacte con el administrador." });
            }

            return Json(new { exito = true, mensaje = "Mensaje enviado con exito." });
        }
    }
}