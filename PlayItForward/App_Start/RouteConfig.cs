// <copyright file="RouteConfig.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Web.Mvc;
using System.Web.Routing;

namespace PiF
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("New", "new", new { controller = "PiF", action = "New" });
            routes.MapRoute(
                "Complete",
                "pif/{thingID}/complete", 
                new { controller = "PiF", action = "Complete", thingID = UrlParameter.Optional });

            routes.MapRoute(
                "Delete",
                "pif/{thingID}/delete", 
                new { controller = "PiF", action = "delete", thingID = UrlParameter.Optional });

            routes.MapRoute(
                "Edit", "pif/{thingID}/edit", new { controller = "PiF", action = "Edit", thingID = UrlParameter.Optional });

            routes.MapRoute(
                "PiF", "pif/{thingID}", new { controller = "PiF", action = "View", thingID = UrlParameter.Optional });

            routes.MapRoute("About", "about", new { controller = "Home", action = "About" });

            routes.MapRoute("Rules", "rules", new { controller = "Home", action = "Rules" });

            routes.MapRoute("GetGames", "api/games", new { controller = "Home", action = "GetGames" });

            routes.MapRoute("Points", "rules/points", new { controller = "Home", action = "Points" });
            routes.MapRoute("Exceptions", "rules/exceptions", new { controller = "Home", action = "Exceptions" });
            routes.MapRoute("Support", "support", new { controller = "Home", action = "Support" });
            routes.MapRoute("Me", "user/me", new { controller = "Account", action = "Me" });

            routes.MapRoute(
                "Login", 
                "user/login/{ReturnUrl}", 
                new { controller = "Account", action = "Login", ReturnUrl = UrlParameter.Optional });

            routes.MapRoute("Logout", "user/logout", new { controller = "Account", action = "Logout" });

            routes.MapRoute(
                "User", 
                "user/{username}", 
                new { controller = "Account", action = "User", username = UrlParameter.Optional });

            routes.MapRoute(
                "Index", "giveaways/{page}", new { controller = "Home", action = "Index", page = UrlParameter.Optional });

            routes.MapRoute(
            "Default",
            "{controller}/{action}/{id}",
            new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}