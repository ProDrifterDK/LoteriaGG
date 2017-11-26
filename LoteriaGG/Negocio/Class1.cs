using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Datos.SqlData;

namespace Negocio
{
    public static class Class1
    {
        public static string CrearUsuario(string usuario, string pass, string email, string confirmPass, string nombre, string apellido, string nombreDeInvocador)
        {
            if(pass == confirmPass)
            {
                return DatosPersonas.CrearUsuario(usuario, pass, nombre, apellido, email, nombreDeInvocador);
            }
            return "Contraseñas no coinciden";
        }
        

        public static TBL_USUARIO Verificar(string usu, string codigo)
        {
            return DatosPersonas.Verificar(usu, codigo);
        }

        public static TBL_USUARIO ObtenerCuenta(string usu)
        {
            return DatosPersonas.ObtenerUsuario(usu);
        }

        public static int CambiarContrasena(string usu, string nC, string nCr, string aC)
        {
            if(nC == nCr)
            {
                var ret = DatosPersonas.CambiarContrasena(usu, nC, aC);
                if (ret)
                    return 0;

                return 1;
            }
            else
            {
                var ret = DatosPersonas.CambiarContrasena(usu, nC, aC, false);
                if (ret)
                    return 3;
                return 2;
            }
        }

        public static bool EditarPerfil(string ac, string nombre, string apellido, string nombreInvocador)
        {
            if(nombre.Length == 1 || apellido.Length == 1 || nombreInvocador.Length == 1)
            {
                return false;
            }
            return DatosPersonas.EditarPerfil(ac, nombre??"", apellido??"", nombreInvocador??"");
        }

        public static TBL_USUARIO Login(string argUsuario, string argPass)
        {
            using (var db = new LoteriaGGEntities())
            {
                return db.TBL_USUARIO.FirstOrDefault(o => o.USU_ACCOUNT == argUsuario && o.USU_PASSWORD == argPass);
            }
        }
    }
}
