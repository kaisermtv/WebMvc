using Microsoft.Practices.Unity;
using Quartz;

namespace WebMvc.Web.Application.ScheduledJobs
{
    public static class ScheduledRunner
    {
        public static void Run(IUnityContainer container)
        {
            // Resolving IScheduler instance
            var scheduler = container.Resolve<IScheduler>();

            

            // Starting scheduler
            scheduler.Start();
        }
    }
}