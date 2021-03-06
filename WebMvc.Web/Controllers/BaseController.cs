﻿namespace WebMvc.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Domain.Constants;
    using WebMvc.Domain.DomainModel.Entities;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.Areas.Admin.ViewModels;
    using WebMvc.Utilities;
    using System.Web.Mvc.Filters;
    using WebMvc.Web.ViewModels;

    public class BaseController : Controller
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IMembershipService MembershipService;
        protected readonly ILocalizationService LocalizationService;
        protected readonly ISettingsService SettingsService;
        protected readonly ILoggingService LoggingService;
        protected readonly ICacheService CacheService;

        protected MembershipUser LoggedOnReadOnlyUser;
        protected Guid UsersRole;

        public BaseController()
        {
            UnitOfWorkManager = ServiceFactory.Get<IUnitOfWorkManager>();
            MembershipService = ServiceFactory.Get<IMembershipService>();
            LocalizationService = ServiceFactory.Get<ILocalizationService>();
            SettingsService = ServiceFactory.Get<ISettingsService>();
            CacheService = ServiceFactory.Get<ICacheService>();
            LoggingService = ServiceFactory.Get<ILoggingService>();
        }

        public BaseController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService,ILocalizationService localizationService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            LocalizationService = localizationService;
            SettingsService = settingsService;
            CacheService = cacheService;
            LoggingService = loggingService;
            
                //UsersRole = LoggedOnReadOnlyUser.ro
                //UsersRole = LoggedOnReadOnlyUser == null ? RoleService.GetRole(AppConstants.GuestRoleName, true) : LoggedOnReadOnlyUser.Roles.FirstOrDefault();

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
        

        protected internal ActionResult ErrorToHomePage(string errorMessage)
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

        protected internal PageingViewModel CalcPaging(int limit,int? page,int count)
        {
            var paging = new PageingViewModel
            {
                Count = count,
                Page = page ?? 1,
                MaxPage = (count / limit) + ((count % limit > 0) ? 1 : 0),
            };

            if (paging.Page > paging.MaxPage) paging.Page = paging.MaxPage;

            return paging;
        }
    }
}