using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SqlData;

namespace Negocio
{
    public static class Home
    {
        public static string Ganador(int tipo)
        {
            using(var db = new LoteriaGGEntities())
            {
                return db.TBL_HOME.FirstOrDefault(o => o.HM_ID == tipo).HM_TXT_GANADORES;
            }
        }
    }
}
