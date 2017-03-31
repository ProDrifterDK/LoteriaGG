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
    public static class DatosAdmin
    {
        public static long IsAdmin(string usuario)
        {
            try
            {
                using (var db = new LOTERIA_GGEntities())
                {
                    var user = db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == usuario);
                    if(user == null)
                    {
                        throw new Exception();
                    }
                    var userAdmin = db.TBL_ADMIN.FirstOrDefault(o => o.USU_ID == user.USU_ID);

                    if(userAdmin != null)
                    {
                        return user.USU_ID;
                    }
                }
            }
            catch (Exception)
            {

            }

            return -1;
        }

        public static bool IniciarTorneo(long UsrId, DateTime inicio, DateTime fin)
        {
            try
            {
                using(var db = new LOTERIA_GGEntities())
                {
                    var newSort = new TBL_SORTEO { SOR_LLENO = false};
                    
                    newSort.SOR_CREADO_POR = UsrId;
                    newSort.SOR_FECHA_FIN = fin;
                    newSort.SOR_FECHA_INICIO = inicio;

                    db.TBL_SORTEO.Add(newSort);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public static long SeleccionarGanador(long idSorteo)
        {
            Random rand = new Random();
            using(var db = new LOTERIA_GGEntities())
            {
                var participantes = db.NUB_SORTEO_USUARIO.Where(o => o.SOR_ID == idSorteo).ToList();
                return participantes[rand.Next(0, participantes.Count)]?.USU_ID ?? 0;
            }
        }
    }
}
