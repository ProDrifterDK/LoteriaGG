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

                    #region Añadir Usuario
                    //var inscripciones = db.NUB_SORTEO_USUARIO.Where(o => o.USU_ID == usuario.USU_ID && o.SOR_ID == sorteo.SOR_ID).ToList();
                    //if(inscripciones.Count > 3)
                    //{
                    //    throw new Exception("Ya esta 3 veces en este sorteo.");
                    //}
                    var nubSor = new NUB_SORTEO_USUARIO { SOR_ID = sorteo.SOR_ID, USU_ID = usuario.USU_ID};


                    db.NUB_SORTEO_USUARIO.Add(nubSor);
                    //if (sorteo.USU_ID_1 == null)
                    //{
                    //    sorteo.USU_ID_1 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO = usuario;
                    //}
                    //else if (sorteo.USU_ID_2 == null)
                    //{
                    //    sorteo.USU_ID_2 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO1 = usuario;
                    //}
                    //else if (sorteo.USU_ID_3 == null)
                    //{
                    //    sorteo.USU_ID_3 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO2 = usuario;
                    //}
                    //else if (sorteo.USU_ID_4 == null)
                    //{
                    //    sorteo.USU_ID_4 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO3 = usuario;
                    //}
                    //else if (sorteo.USU_ID_5 == null)
                    //{
                    //    sorteo.USU_ID_5 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO4 = usuario;
                    //}
                    //else if (sorteo.USU_ID_6 == null)
                    //{
                    //    sorteo.USU_ID_6 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO5 = usuario;
                    //}
                    //else if (sorteo.USU_ID_7 == null)
                    //{
                    //    sorteo.USU_ID_7 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO6 = usuario;
                    //}
                    //else if (sorteo.USU_ID_8 == null)
                    //{
                    //    sorteo.USU_ID_8 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO7 = usuario;
                    //}
                    //else if (sorteo.USU_ID_9 == null)
                    //{
                    //    sorteo.USU_ID_9 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO8 = usuario;
                    //}
                    //else if (sorteo.USU_ID_10 == null)
                    //{
                    //    sorteo.USU_ID_10 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO9 = usuario;
                    //}
                    //else if (sorteo.USU_ID_11 == null)
                    //{
                    //    sorteo.USU_ID_11 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO10 = usuario;
                    //}
                    //else if (sorteo.USU_ID_12 == null)
                    //{
                    //    sorteo.USU_ID_12 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO11 = usuario;
                    //}
                    //else if (sorteo.USU_ID_13 == null)
                    //{
                    //    sorteo.USU_ID_13 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO12 = usuario;
                    //}
                    //else if (sorteo.USU_ID_14 == null)
                    //{
                    //    sorteo.USU_ID_14 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO13 = usuario;
                    //}
                    //else if (sorteo.USU_ID_15 == null)
                    //{
                    //    sorteo.USU_ID_15 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO14 = usuario;
                    //}
                    //else if (sorteo.USU_ID_16 == null)
                    //{
                    //    sorteo.USU_ID_16 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO15 = usuario;
                    //}
                    //else if (sorteo.USU_ID_17 == null)
                    //{
                    //    sorteo.USU_ID_7 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO16 = usuario;
                    //}
                    //else if (sorteo.USU_ID_18 == null)
                    //{
                    //    sorteo.USU_ID_18 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO17 = usuario;
                    //}
                    //else if (sorteo.USU_ID_19 == null)
                    //{
                    //    sorteo.USU_ID_19 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO18 = usuario;
                    //}
                    //else if (sorteo.USU_ID_20 == null)
                    //{
                    //    sorteo.USU_ID_20 = usuario.USU_ID;
                    //    sorteo.TBL_USUARIO19 = usuario;
                    //    sorteo.SOR_LLENO = true;
                    //}
                    #endregion

                    db.SaveChanges();
                    return "Inscrito!";
                }
            }
            catch(Exception ex)
            {
                return "Hubo un error. Contacte con un administrador";
            }

            return "Error";
        }
    }
}
