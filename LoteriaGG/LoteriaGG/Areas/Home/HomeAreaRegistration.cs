using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGG.Areas.Home
{
    public class HomeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Home";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Home_default",
                "Home/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "LoteriaGG.Areas.Home.Controllers" }
            );
        }
    }
}
