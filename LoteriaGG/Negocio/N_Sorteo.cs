using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;

namespace Negocio
{
    public static class N_Sorteo
    {
        public static string Inscripcion(string us)
        {
            return DatosSorteo.Inscripcion(us);
        }
    }
}
