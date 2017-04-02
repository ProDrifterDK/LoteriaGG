using System;
using System.Data;
using System.Data.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio;
using Datos.SqlData;

namespace LoteriaGG.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if(Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password, string email = "", string confirmPassword = "", string type = "login", string Nombre = "", string Apellido ="", string NombreDeInvocador = "")
        {
            if(type == "login")
            {
                var user = Log(username, password);
                if (user != null)
                {
                    Session["LogedIn"] = "True";
                    Session["User"] = user.USU_ACCOUNT;
                }
            }
            else
            {
                var reg = Register(username, password, email, confirmPassword, Nombre, Apellido, NombreDeInvocador);
                if(reg == "success")
                {
                    return RedirectToAction("Verification", "Home", new { us="", verif="" });
                }
                else
                {
                    ViewBag.Mensaje = reg;
                }
            }
            return View();
        }

        TBL_USUARIO Log(string user, string pass)
        {
            return Class1.LogIn(user, pass);
        }

        string Register(string username, string password, string email, string confirmPassword, string nombre, string apellido, string nombreDeInvocador)
        {
            return Class1.CrearUsuario(username, password, email, confirmPassword, nombre, apellido, nombreDeInvocador);
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session["LogedIn"] = "";
            return RedirectToAction("Index");
        }

        public ActionResult Verification(string us, string verif)
        {
            if(Session["LogedIn"] == null)
            {
                Session["LogedIn"] = "";
            }else if(Session["LogedIn"].ToString() != "")
            {
                return RedirectToAction("Index", "Home", new { });
            }
            if (us == "" && verif == "")
                ViewBag.ret = null;
            else
            {
                if(us != null && verif != null)
                {
                    Session["LogedIn"] = "True";
                    Session["User"] = Class1.Verificar(us, verif).USU_ACCOUNT;
                    ViewBag.ret = true;
                }
                ViewBag.ret = null;
            }
            return View();
        }
    }
}