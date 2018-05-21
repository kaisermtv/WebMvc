﻿namespace WebMvc.Web.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;

    public class AboutController : BaseController
    {
        public AboutController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        // GET: About
        public ActionResult Index()
        {
            return View();
        }
    }
}