using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class DashboardController : BaseAdminController
    {
        public DashboardController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        [ChildActionOnly]
        public PartialViewResult MainAdminNav()
        {
            var viewModel = new MainDashboardNavViewModel
            {
                PrivateMessageCount = 0,
                ModerateCount = 0
            };
            return PartialView(viewModel);
        }

    }
}