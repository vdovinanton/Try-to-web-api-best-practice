﻿using NLog;
using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplicationExercise.Core;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Filters.Add(new ExceptionHandler());
            GlobalConfiguration.Configuration.Filters.Add(new ExecutionTimeAttribute());

            NinjectHttpContainer.RegisterModules(NinjectHttpModules.Modules);
            
            //Logger.Instance.Information("Application started");
            _logger.Info("Application started");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }
    }
}
