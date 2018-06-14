using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.ViewModels;

namespace WebMvc.Web.Controllers
{
    public class AjaxController : BaseController
    {
        public AjaxController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        public ActionResult TopShowroom()
        {
            var viewModel = new AjaxShowroomViewModel
            {
                Showrooms = new List<AjaxShowroomItemViewModel>(),
            };

            var ShowroomCount = SettingsService.GetSetting("ShowroomCount");
            int count = 0;
            try
            {
                count = int.Parse(ShowroomCount);
            }
            catch { }

            for (int i = 0; i < count; i++)
            {
                viewModel.Showrooms.Add(new AjaxShowroomItemViewModel
                {
                    Addren = SettingsService.GetSetting("Showroom[" + i + "].Address"),
                    iFrameMap = SettingsService.GetSetting("Showroom[" + i + "].iFrameMap"),
                });
            }

            return PartialView(viewModel);
        }


    }
}