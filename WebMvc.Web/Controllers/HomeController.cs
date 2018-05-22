namespace WebMvc.Web.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;

    public class HomeController : BaseController
    {

        public HomeController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult Slider()
        {


            return PartialView();
        }
    }
}
