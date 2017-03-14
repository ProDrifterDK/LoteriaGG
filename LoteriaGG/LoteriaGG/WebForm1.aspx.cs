using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;

namespace LoteriaGG
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectDb();
        }

        public void ConnectDb()
        {

            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["LOTERIA_GGConectionString"].ConnectionString);
            con.Open();
            if(con.State == System.Data.ConnectionState.Open)
            {
                DisplayMessage(this, "Connection to de database succesful");
            }
            else
            {
                DisplayMessage(this, "Fail");
            }
        }

        static public void DisplayMessage(Control page, string msg)
        {
            string myScript = String.Format("alert('{0}')", msg);
            ScriptManager.RegisterStartupScript(page, page.GetType(), "MyScript", myScript, true);
        }
    }
}