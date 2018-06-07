namespace WebMvc.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.ViewModels;

    public class PluginController : BaseController
    {
        private readonly IProductSevice _productSevice;

        public PluginController() : base()
        {
            _productSevice = ServiceFactory.Get<IProductSevice>();
        }

        public PluginController(IProductSevice productSevice, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _productSevice = productSevice;
        }

        // GET: Plugin
        public ActionResult ComputerBuilding()
        {
            var viewModel = new PluginComputerBuildingViewModel();
            viewModel.ProductClass = new List<PluginProductClassViewModel>();

            dynamic value = ThemesSetting.getValue("ComputerBuilding");

            foreach(var it in value)
            {
                var id = new Guid((string)it.Value);

                var cat = _productSevice.GetProductClass(id);
                if (cat == null) continue;

                var a = new PluginProductClassViewModel
                {
                    Id = id,
                    Name = cat.Name.ToUpper()
                };

                viewModel.ProductClass.Add(a);
            }


            return View(viewModel);
        }
    }
}