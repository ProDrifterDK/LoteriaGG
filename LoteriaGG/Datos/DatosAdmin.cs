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
        public static bool IsAdmin(string usuario)
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
                        return true;
                    }
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}
