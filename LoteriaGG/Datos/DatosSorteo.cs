using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SqlData;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System;

namespace Datos
{
    public static class DatosSorteo
    {
        public static string Inscripcion(string user)
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    var usuario = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);

                    var sorteo = db.TBL_SORTEO.FirstOrDefault(o => !o.SOR_LLENO);

                    if(sorteo == null)
                    {
                        throw new Exception("Lo sentimos, no hay sorteos disponibles");
                    }

                    //var inscripciones = db.NUB_SORTEO_USUARIO.Where(o => o.USU_ID == usuario.USU_ID && o.SOR_ID == sorteo.SOR_ID).ToList();
                    //if(inscripciones.Count > 3)
                    //{
                    //    throw new Exception("Ya esta 3 veces en este sorteo.");
                    //}
                    var nubSor = new NUB_SORTEO_USUARIO { SOR_ID = sorteo.SOR_ID, USU_ID = usuario.USU_ID};


                    db.NUB_SORTEO_USUARIO.Add(nubSor);
                    db.SaveChanges();
                    return "Inscrito!";
                }
            }
            catch(Exception ex)
            {
                return "Hubo un error. Contacte con un administrador";
            }
        }
    }
}
