using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace RP.Sistema.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{format}",
                defaults: new { id = RouteParameter.Optional, format = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Ticket_List",
                url: "Tickets",
                defaults: new { controller = "Ticket", action = "Index" }
            );

            routes.MapRoute(
                name: "Ticket_Details",
                url: "Ticket/{tipo}/{id}",
                defaults: new { controller = "Ticket", action = "Detail", tipo = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}