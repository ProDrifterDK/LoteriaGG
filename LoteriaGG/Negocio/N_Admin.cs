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
        public static bool IsAdmin(string user)
        {
            return DatosAdmin.IsAdmin(user);
        }
    }
}
