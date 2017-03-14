using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Connection
{
    public class Conexion
    {
        public SqlConnection conn;
        public SqlCommand command;

        public void IniciarConexion()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LOTERIA_GGConectionString"].ToString());
            conn.Open();
        }

        public void FinalizarConexion()
        {
            conn.Close();
            //command.Dispose();
        }

        public void IniciarSQLCommand(string usp_procedure)
        {
            command = new SqlCommand(usp_procedure, conn);
            command.CommandType = CommandType.StoredProcedure;
        }

        public void IniciarSqlCommandQuery(string u_query)
        {
            command = new SqlCommand(u_query, conn);
            command.CommandType = CommandType.Text;
        }

        public IDataReader ExecuteReader()
        {
            IDataReader dr = command.ExecuteReader();
            return dr;
        }

        public IDataReader ExecuteReader(SqlParameter[] param)
        {
            command.Parameters.AddRange(param);
            IDataReader dr = command.ExecuteReader();
            return dr;
        }

        public void ExecuteNonQuery()
        {
            command.ExecuteReader();
        }

        public void ExecuteNonQuery(SqlParameter[] param)
        {
            command.Parameters.AddRange(param);
            command.ExecuteReader();
        }
    }
}
