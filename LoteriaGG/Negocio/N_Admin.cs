using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;

namespace Negocio
{
    public static class N_Admin
    {
        public static long IsAdmin(string user)
        {
            return DatosAdmin.IsAdmin(user);
        }
        public static bool IniciarToreneo(long UsrID, string fin)
        {
            var i = DateTime.Now;
            var f = DateTime.ParseExact(fin, "MM-dd-yyyy HH:mm",null);

            return DatosAdmin.IniciarTorneo(UsrID, i, f);
        }
    }
}
