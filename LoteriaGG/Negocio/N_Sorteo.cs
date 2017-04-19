using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Datos.SqlData;

namespace Negocio
{
    public static class N_Sorteo
    {
        public static string Inscripcion(string us, long sorteo)
        {
            return DatosSorteo.Inscripcion(us, sorteo);
        }

        public static List<TBL_SORTEO> Sorteos()
        {
            return DatosSorteo.ObtenerSorteo();
        }
    }
}
