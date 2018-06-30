using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    public class AdminMembersController : BaseAdminController
    {
        public AdminMembersController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        // GET: Admin/AdminMembership
        public ActionResult Index(int? p)
        {
            int limit = 10;
            var count = MembershipService.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new AdminListMembersViewModel
            {
                Paging = Paging,
                ListMembers = MembershipService.GetList(limit, Paging.Page)
            };

            return View(viewModel);
        }

        #region Create Accout

        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Edit info Accout
        public ActionResult Edit()
        {
            return View();
        }
        #endregion

        #region New pass
        public ActionResult NewPass()
        {
            return View();
        }
        #endregion

        #region change info
        public ActionResult ChangeInfo()
        {
            return View();
        }
        #endregion

        #region change pass
        public ActionResult ChangePass()
        {
            return View();
        }
        #endregion
    }
}