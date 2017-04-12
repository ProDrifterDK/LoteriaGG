using System;
using System.Linq;
using Datos.SqlData;
using System.Net.Mail;
using System.Net;
//Code by Alcelon -- Resyst --
//CODE 3
namespace Datos
{
    public static class DatosPersonas
    {
        public static string CrearUsuario(string usuario, string pass, string nombre, string apellido, string email, string nombreDeInvocador)
        {
            
            return "success";
        }



        public static TBL_USUARIO Verificar(string user, string verification)
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

                    return db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user && o.USU_CODIGO_VERIFICAION == dsa);
                }
            }
            catch(Exception ex)
            {
                return null;
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
