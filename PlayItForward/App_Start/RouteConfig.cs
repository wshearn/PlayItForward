// <copyright file="RouteConfig.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
namespace PiF
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The route config.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("New", "new", new { controller = "PiF", action = "New" });
            routes.MapRoute(
                "Complete", 
                "complete/{thingID}", 
                new { controller = "PiF", action = "Complete", thingID = UrlParameter.Optional });

            routes.MapRoute(
                "Delete", 
                "delete/{thingID}", 
                new { controller = "PiF", action = "delete", thingID = UrlParameter.Optional });

            routes.MapRoute(
                "Edit", "edit/{thingID}", new { controller = "PiF", action = "Edit", thingID = UrlParameter.Optional });

            routes.MapRoute(
                "PiF", "pif/{thingID}", new { controller = "PiF", action = "View", thingID = UrlParameter.Optional });

            routes.MapRoute("About", "about", new { controller = "Home", action = "About" });

            routes.MapRoute("Rules", "rules", new { controller = "Home", action = "Rules" });

            routes.MapRoute("Points", "rules/points", new { controller = "Home", action = "Points" });
            routes.MapRoute("Exceptions", "rules/exceptions", new { controller = "Home", action = "Exceptions" });

            routes.MapRoute("Me", "me", new { controller = "Account", action = "Me" });

            routes.MapRoute(
                "Login", 
                "login/{ReturnUrl}", 
                new { controller = "Account", action = "Login", ReturnUrl = UrlParameter.Optional });

            routes.MapRoute("Logout", "logout", new { controller = "Account", action = "Logout" });

            routes.MapRoute(
                "User", 
                "user/{username}", 
                new { controller = "Account", action = "User", username = UrlParameter.Optional });

            routes.MapRoute(
                "Index", "{page}", new { controller = "Home", action = "Index", page = UrlParameter.Optional });

            // routes.MapRoute(
            // "Default",
            // "{controller}/{action}/{id}",
            // new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}