using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGG.Areas.Home
{
    public class LolLAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LoL";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LoL_default",
                "LoL/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "LoteriaGG.Areas.LoL.Controllers" }
            );
        }
    }
}
