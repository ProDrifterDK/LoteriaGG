using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SqlData;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

namespace Datos 
{
    public static class DatosPersonas
    {
        public static string CrearUsuario(string usuario, string pass, string nombre, string apellido, string email, string nombreDeInvocador)
        {
            try
            {
                using(var dc = new LOTERIA_GGEntities())
                {
                    if(dc.TBL_USUARIO.FirstOrDefault(o=>o.USU_ACCOUNT == usuario) != null)
                    {
                        throw new Exception("Usuario ya registrado");
                    }
                    else if(dc.TBL_USUARIO.FirstOrDefault(o=>o.USU_EMAIL == email) != null)
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
                    dc.SaveChanges();
                    SendEmailConfirmation(email, usuario, activationCode.ToString(), nombre);
                }
            }
            catch(Exception ex)
            {
                return ex.Message + ex.InnerException ?? "";
            }
            return "success";
        }

        private static void SendEmailConfirmation(string to, string username, string confirmationToken, string name)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(new MailAddress(to));
            mm.From = new MailAddress("loteriaggnoreply@gmail.com");
            mm.Body = "<h3>Hola "+ name + "</h3>+ <p>Para verificar el email debes presionar el siguiente link</p><a href=" + "\"http://localhost:54639/Home/Verification?us="+ username + "&verif=" + confirmationToken + "\""+">Preciona aquí para verificar</a>"
                +"<p>PD: El CEO Alan quiere chupar pico</p>";
            mm.IsBodyHtml = true;
            mm.Subject = "Verification";
            SmtpClient smcl = new SmtpClient();
            smcl.Host = "smtp.gmail.com";
            smcl.Port = 587;
            smcl.Credentials = new NetworkCredential("loteriaggnoreply@gmail.com", "123456789ad");
            smcl.EnableSsl = true;
            smcl.Send(mm);
        }

        public static bool Verificar(string user, string verification)
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    var dsa = Guid.Parse(verification);
                    if (db.TBL_USUARIO.FirstOrDefault(o=> o.USU_ACCOUNT == user && o.USU_CODIGO_VERIFICAION == dsa) != null)
                    {
                        db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user && o.USU_CODIGO_VERIFICAION == dsa).USU_VERIFICADO = true;
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        
        public static TBL_USUARIO LogIn(string usu, string pass)
        {
            TBL_USUARIO ret = null;
            try
            {
                using(var dc = new LOTERIA_GGEntities())
                {
                    ret = dc.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu && o.USU_PASSWORD == pass && o.USU_VERIFICADO == true );
                }
            }
            catch(Exception ex)
            {

            }

            return ret;
        }

        public static TBL_USUARIO ObtenerUsuario(string usu)
        {
            TBL_USUARIO ret = null;
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    ret = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu);
                }
            }catch(Exception ex)
            {

            }
            return ret;
        }

        public static bool CambiarContrasena(string usu, string nC, string aC, bool cambio = true)
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    if(db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu).USU_PASSWORD == aC)
                    {
                        if (cambio)
                        {
                            db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usu).USU_PASSWORD = nC;
                            db.SaveChanges();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EditarPerfil(string ac, string nombre = "", string apellido = "", string nombreInvocador = "")
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == ac).USU_NOMBRE = nombre == "" ? db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == ac).USU_NOMBRE : nombre;
                    db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == ac).USU_APELLIDO = apellido == "" ? db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == ac).USU_APELLIDO : apellido;
                    db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == ac).USU_SUMMONER = nombreInvocador == "" ? db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == ac).USU_SUMMONER : nombreInvocador;
                    db.SaveChanges();
                }
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
    }

    public class ApplicationUser
    {
        public string Email { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
