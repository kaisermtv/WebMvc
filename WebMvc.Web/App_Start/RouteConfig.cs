using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebMvc.Domain.Constants;

namespace WebMvc.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "category",
                url: SiteConstants.Instance.CategoryUrlIdentifier,
                defaults: new { controller = "Category", action = "Index" }
            );

            routes.MapRoute(
                "categoryUrls", // Route name
                string.Concat(SiteConstants.Instance.CategoryUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Category", action = "ShowBySlug", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "topicShowUrls", // Route name
                string.Concat(SiteConstants.Instance.CategoryUrlIdentifier, "/{catslug}/{slug}"), // URL with parameters
                new { controller = "Topic", action = "ShowBySlug", catslug = UrlParameter.Optional, slug = UrlParameter.Optional } // Parameter defaults
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            
        }
    }
}
