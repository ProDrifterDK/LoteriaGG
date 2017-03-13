using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;




namespace Datos.Connection
{
    public class DataContext : IDisposable
    {
        #region Atributos.
        public SqlParameterCollection Parameters { get; set; }
        public DbConnection Connection { get; set; }
        public string Catalog { get; set; }
        public string Command { get; set; }
        public CommandType CommandType { get; set; }
        #endregion Atributos.

        #region Constructores.
        public DataContext()
        {

            this.Connection = null;
            this.Command = "";
            this.CommandType = CommandType.Text;
            this.Parameters = (SqlParameterCollection)typeof(SqlParameterCollection)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                ).Invoke(null);
        }

        public DataContext(string Catalog, string Command, CommandType CommandType)
        {
            this.Connection = null;
            this.Catalog = Catalog;
            this.Command = Command;
            this.CommandType = CommandType;
            this.Parameters = (SqlParameterCollection)typeof(SqlParameterCollection)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                ).Invoke(null);
        }

        public DataContext(DbConnection Connection, string Command, CommandType CommandType)
        {
            this.Connection = Connection;
            this.Catalog = Catalog;
            this.Command = Command;
            this.CommandType = CommandType;
            this.Parameters = (SqlParameterCollection)typeof(SqlParameterCollection)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                ).Invoke(null);
        }
        #endregion Constructores.

        #region ExecuteQuery.
        public DataTable ExecuteQuery(int TableIndex = 0)
        {
            try
            {
                string ConnectionString = this.Connection != null ? this.Connection.ConnectionString : ConfigurationManager.ConnectionStrings[this.Catalog.ToUpper()].ToString();

                using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    sqlCon.Open();

                    using (SqlCommand SqlCom = new SqlCommand(this.Command, sqlCon))
                    {
                        if (this.Parameters != null)
                        {
                            foreach (SqlParameter Parameter in this.Parameters)
                                SqlCom.Parameters.AddWithValue(Parameter.ParameterName, Parameter.Value);
                        }

                        SqlCom.CommandType = this.CommandType;

                        this.LogSQLCommand(SqlCom);

                        using (SqlDataAdapter SDA = new SqlDataAdapter(SqlCom))
                        {
                            DataSet DS = new DataSet();

                            SDA.Fill(DS);

                            if (DS != null && DS.Tables.Count > TableIndex && DS.Tables[TableIndex].Rows.Count > 0)
                                return DS.Tables[TableIndex];
                            else return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0}", ex.Message));
                throw new Exception("Error al ejecutar [ExecuteQuery].", ex);
            }
        }
        #endregion ExecuteQuery.

        #region ExecuteQuery<T>.
        public List<T> ExecuteQuery<T>(int TableIndex = 0) where T : new()
        {
            try
            {
                string ConnectionString = this.Connection != null ? this.Connection.ConnectionString : ConfigurationManager.ConnectionStrings[this.Catalog.ToUpper()].ToString();

                using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
                {
                    SqlCon.Open();

                    using (SqlCommand SqlCom = new SqlCommand(this.Command, SqlCon))
                    {
                        if (this.Parameters != null)
                        {
                            foreach (SqlParameter Parameter in this.Parameters)
                            {
                                SqlCom.Parameters.AddWithValue(Parameter.ParameterName, Parameter.Value);
                            }
                        }

                        SqlCom.CommandType = this.CommandType;

                        this.LogSQLCommand(SqlCom);

                        using (SqlDataAdapter SDA = new SqlDataAdapter(SqlCom))
                        {
                            DataSet DS = new DataSet();

                            SDA.Fill(DS);

                            if (DS != null && DS.Tables.Count > TableIndex && DS.Tables[TableIndex].Rows.Count > 0)
                            {
                                return DS.Tables[TableIndex].ToList<T>();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0}", ex.Message));
                throw new Exception("Error al ejecutar ExecuteQuery<T>.", ex);
            }
        }
        #endregion ExecuteQuery<T>.

        #region ExecuteQuery DataTableCollection.
        public DataTableCollection ExecuteQuery()
        {
            try
            {
                string ConnectionString = this.Connection != null ? this.Connection.ConnectionString : ConfigurationManager.ConnectionStrings[this.Catalog.ToUpper()].ToString();

                using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    sqlCon.Open();

                    using (SqlCommand SqlCom = new SqlCommand(this.Command, sqlCon))
                    {
                        if (this.Parameters != null)
                        {
                            foreach (SqlParameter Parameter in this.Parameters)
                                SqlCom.Parameters.AddWithValue(Parameter.ParameterName, Parameter.Value);
                        }

                        SqlCom.CommandType = this.CommandType;

                        this.LogSQLCommand(SqlCom);

                        using (SqlDataAdapter SDA = new SqlDataAdapter(SqlCom))
                        {
                            DataSet DS = new DataSet();

                            SDA.Fill(DS);

                            if (DS != null && DS.Tables.Count > 0)
                                return DS.Tables;
                            else return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0}", ex.Message));
                throw new Exception("Error al ejecutar ExecuteQuery.", ex);
            }
        }
        #endregion ExecuteQuery DataTableCollection.

        #region ExecuteNonQuery.
        public Boolean ExecuteNonQuery()
        {
            try
            {
                string ConnectionString = this.Connection != null ? this.Connection.ConnectionString : ConfigurationManager.ConnectionStrings[this.Catalog.ToUpper()].ToString();

                using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
                {
                    SqlCon.Open();

                    using (SqlCommand SqlCom = new SqlCommand(this.Command, SqlCon))
                    {
                        if (this.Parameters != null)
                        {
                            foreach (SqlParameter Parameter in this.Parameters)
                            {
                                SqlCom.Parameters.AddWithValue(Parameter.ParameterName, Parameter.Value);
                            }
                        }

                        SqlCom.CommandType = this.CommandType;

                        this.LogSQLCommand(SqlCom);

                        if (SqlCom.ExecuteNonQuery() >= 0)
                        {
                            SqlCon.Close();

                            return true;
                        }
                        else
                        {
                            SqlCon.Close();

                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0}", ex.Message));
                throw new Exception("Error al ejecutar ExecuteNonQuery.", ex);
            }
        }
        #endregion ExecuteNonQuery.

        #region LogSQLCommand.
        private void LogSQLCommand(SqlCommand Command)
        {
            string StrCommand = "";

            if (Command.CommandType == CommandType.StoredProcedure)
            {
                StrCommand = string.Format("{0}.{1}", Command.Connection.Database, Command.CommandText);

                foreach (SqlParameter Parameter in Command.Parameters)
                {
                    string Value = "";

                    switch (Parameter.SqlDbType)
                    {
                        case SqlDbType.Int:
                        case SqlDbType.Float:
                            Value = Parameter.Value != null ? Parameter.Value.ToString() : DBNull.Value.ToString();
                            StrCommand += string.Format(" {0},", Value);

                            break;
                        case SqlDbType.Text:
                        case SqlDbType.VarChar:
                        case SqlDbType.Char:
                        case SqlDbType.DateTime:
                        case SqlDbType.NVarChar:
                            Value = Parameter.Value != null ? Parameter.Value.ToString() : "";
                            StrCommand += string.Format(" '{0}',", Value);

                            break;
                        case SqlDbType.VarBinary:
                            StrCommand += string.Format(" {0},", DBNull.Value);

                            break;
                        default:
                            Value = Parameter.Value != null ? Parameter.Value.ToString() : "";
                            StrCommand += string.Format(" '{0}',", Value);

                            break;
                    }
                }

                if (Command.Parameters.Count > 0) StrCommand = StrCommand.Substring(0, StrCommand.Length - 1);
            }
            else if (Command.CommandType == CommandType.Text)
            {
                StrCommand = Command.CommandText;

                foreach (SqlParameter Parameter in Command.Parameters)
                {
                    switch (Parameter.SqlDbType)
                    {
                        case SqlDbType.Int:
                        case SqlDbType.Float:
                            StrCommand = StrCommand.Replace(string.Format("@{0}", Parameter.ParameterName), Parameter.Value.ToString());

                            break;
                        case SqlDbType.Text:
                        case SqlDbType.VarChar:
                        case SqlDbType.Char:
                        case SqlDbType.DateTime:
                        case SqlDbType.NVarChar:
                        case SqlDbType.UniqueIdentifier:
                            StrCommand = StrCommand.Replace(string.Format("@{0}", Parameter.ParameterName), string.Format("'{0}'", Parameter.Value.ToString()));

                            break;
                        default:
                            StrCommand = StrCommand.Replace(string.Format("@{0}", Parameter.ParameterName), string.Format("'{0}'", Parameter.Value.ToString()));

                            break;
                    }
                }
            }
            else
            {
                StrCommand = Command.CommandText;
            }

            Debug.WriteLine(string.Format("\n{0}\n", StrCommand));
        }
        #endregion LogSQLCommand.

        #region Dispose.
        public void Dispose()
        {
            this.Parameters = null;
            this.Connection = null;
            this.Catalog = "";
            this.Command = "";
            this.CommandType = CommandType.Text;
        }
        #endregion Dispose
    }
}
