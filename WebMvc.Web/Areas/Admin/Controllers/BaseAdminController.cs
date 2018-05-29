using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Utilities;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class BaseAdminController : Controller
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IMembershipService MembershipService;
        protected readonly ILocalizationService LocalizationService;
        protected readonly ISettingsService SettingsService;
        protected readonly ILoggingService LoggingService;

        protected MembershipUser LoggedOnReadOnlyUser;
        protected Guid UsersRole;

        public BaseAdminController()
        {
            UnitOfWorkManager = ServiceFactory.Get<IUnitOfWorkManager>();
            MembershipService = ServiceFactory.Get<IMembershipService>();
            LocalizationService = ServiceFactory.Get<ILocalizationService>();
            SettingsService = ServiceFactory.Get<ISettingsService>();
            LoggingService = ServiceFactory.Get<ILoggingService>();
        }

        public BaseAdminController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            LocalizationService = localizationService;
            SettingsService = settingsService;
            LoggingService = loggingService;
            
        }

       
        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;


        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            LoggedOnReadOnlyUser = UserIsAuthenticated ? MembershipService.GetUser(Username) : null;

            if (!Username.IsNullEmpty() && LoggedOnReadOnlyUser == null)
            {
                System.Web.Security.FormsAuthentication.SignOut();
                filterContext.Result = RedirectToAction("index", "Home");
            }
        }

        internal ActionResult ErrorToHomePage(string errorMessage)
        {
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = errorMessage,
                MessageType = GenericMessages.danger
            };
            // Not allowed in here so
            return RedirectToAction("Index", "Home");
        }
    }
}