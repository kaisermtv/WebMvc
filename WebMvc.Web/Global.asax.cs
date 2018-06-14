using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.IOC;
using WebMvc.Utilities;
using WebMvc.Web.Application;
using WebMvc.Web.Application.ScheduledJobs;
using WebMvc.Web.Application.ViewEngine;

namespace WebMvc.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public ILoggingService LoggingService => ServiceFactory.Get<ILoggingService>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            var unityContainer = UnityHelper.Start();
            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LoggingService.Initialise(ConfigUtils.GetAppSettingInt32("LogFileMaxSizeBytes", 10000));
            LoggingService.Error("START APP");

            // Set the view engine
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new WebMvcViewEngine("NewsBlue"));

            
            ScheduledRunner.Run(unityContainer);
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //var entityContext = HttpContext.Current.Items[SiteConstants.Instance.MvcForumContext] as IDatabase;
            //entityContext?.Dispose();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            // Don't flag missing pages or changed urls, as just clogs up the log
            if (!lastError.Message.Contains("was not found or does not implement IController"))
            {
                LoggingService.Error(lastError);
            }
        }
    }
}
