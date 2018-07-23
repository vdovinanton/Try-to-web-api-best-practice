using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //var httpModules = new NinjectHttpModules();
            config.DependencyResolver = new NinjectHttpResolver(NinjectHttpModules.Modules);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            SwaggerConfig.Register(config);
        }
    }
}
