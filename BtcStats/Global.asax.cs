using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BtcStats.Controllers;

namespace BtcStats
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "UserBar4", // Route name
                "sig/getkey/{pool}/{apiKey}", // URL with parameters
                new { controller = "Sig", action = "GetKey" } // Parameter defaults
            );

            routes.MapRoute(
                "UserBar2", // Route name
                "sig/{action}/{key}.png", // URL with parameters
                new { controller = "Sig", mode = ImageType.Sig } // Parameter defaults
            );

            routes.MapRoute(
                "UserBar1", // Route name
                "sig/{action}/{key}", // URL with parameters
                new { controller = "Sig", mode = ImageType.Sig } // Parameter defaults
            );

            routes.MapRoute(
                "UserBar3", // Route name
                "sig/{key}", // URL with parameters
                new { controller = "Sig", action = "Generic", mode = ImageType.Sig } // Parameter defaults
            );

            routes.MapRoute(
                "UserBar5", // Route name
                "avatar/{action}/{key}", // URL with parameters
                new { controller = "Sig", mode = ImageType.Avatar } // Parameter defaults
            );

            routes.MapRoute(
                "UserBar6", // Route name
                "avatar/{key}", // URL with parameters
                new { controller = "Sig", action = "Generic", mode = ImageType.Avatar } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Sig", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}