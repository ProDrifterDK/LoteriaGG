using LoteriaGGNew.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Datos.SqlData;

namespace LoteriaGGNew
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            //if (httpException == null) return;

            if (exception.Message.Contains("Multiple types were found that match the controller named 'Home'."))
            {
                Response.Redirect("~/Choose/Index");
                return;
            }

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Choose");
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("mensaje", "Ha ocurrido un error desconocido. Si esto sigue ocurriendo, por favor envía un mensaje al soporte describiendo tu situación.");

            if (Session["UsuarioLogged"] != null)
            {
                TBL_USUARIO usuario = (TBL_USUARIO)Session["UsuarioLogged"];
                using (var BDD = new LoteriaGGEntities())
                {
                    TBL_BITACORA_ERROR err = new TBL_BITACORA_ERROR
                    {
                        BITERR_INNER_MSG = exception.InnerException?.Message,
                        BITERR_MSG = exception.Message,
                        USU_ID = usuario?.USU_ID,
                    };

                    BDD.TBL_BITACORA_ERROR.Add(err);
                    BDD.Entry(err).State = System.Data.Entity.EntityState.Added;
                    BDD.SaveChanges();
                }
            }

            //if (httpException != null) {
            //    if (httpException.GetHttpCode() == 404) {
            //        routeData.Values["action"] = "HttpError404";
            //    }
            //}
            Server.ClearError();
            Response.Clear();
            IController errorController = new ChooseController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            Response.Redirect("~/Choose/Index?mensaje=Ha ocurrido un error desconocido. Si esto sigue ocurriendo, por favor envía un mensaje al soporte describiendo tu situación.");
        }
    }
}
