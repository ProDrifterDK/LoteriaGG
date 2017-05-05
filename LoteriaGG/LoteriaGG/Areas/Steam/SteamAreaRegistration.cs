using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoteriaGG.Areas.Home
{
    public class SteamAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Steam";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Steam_default",
                "Steam/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "LoteriaGG.Areas.Steam.Controllers" }
            );
        }
    }
}
