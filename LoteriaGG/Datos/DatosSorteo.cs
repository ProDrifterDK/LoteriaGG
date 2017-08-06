using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SqlData;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System;
using System.Collections.Generic;

namespace Datos
{
    public static class DatosSorteo
    {
        public static string Inscripcion(string user, long sorId)
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    var usuario = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == user);

                    var sorteo = db.TBL_SORTEO.FirstOrDefault(o => o.SOR_ID == sorId);

                    if(sorteo == null)
                    {
                        throw new Exception("Lo sentimos, no hay sorteos disponibles");
                    }
                    if(usuario.USU_SOR_DISP == null || usuario.USU_SOR_DISP == 0)
                    {
                        return "No se ha realizaado la inscripción ya que no tienes GGCoins.";
                    }

                    if (!usuario.USU_PAGADO)
                    {
                        return "Para participar en el sorteo debes haber comprado al menos un GGCoin";
                    }

                    //var inscripciones = db.NUB_SORTEO_USUARIO.Where(o => o.USU_ID == usuario.USU_ID && o.SOR_ID == sorteo.SOR_ID).ToList();
                    //if(inscripciones.Count > 3)
                    //{
                    //    throw new Exception("Ya esta 3 veces en este sorteo.");
                    //}
                    var nubSor = new NUB_SORTEO_USUARIO { SOR_ID = sorteo.SOR_ID, USU_ID = usuario.USU_ID};

                    usuario.USU_SOR_DISP--;
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

        public static List<TBL_SORTEO> ObtenerSorteo()
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    return db.TBL_SORTEO.Where(o => o.SOR_FECHA_FIN.Value.AddHours(-1) <= DateTime.Now && !o.SOR_LLENO).ToList();
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
