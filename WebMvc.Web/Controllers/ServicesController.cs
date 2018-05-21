namespace WebMvc.Web.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.ViewModels;
    using Application;
    using WebMvc.Domain.DomainModel.Entities;
    using System;

    public class ServicesController : BaseController
    {
        public ServicesController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }
        // GET: Services
        public ActionResult Index()
        {
            return View();
        }
    }
}