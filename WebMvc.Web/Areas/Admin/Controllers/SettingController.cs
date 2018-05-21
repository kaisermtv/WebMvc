using System;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class SettingController : BaseAdminController
    {
        public SettingController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        public ActionResult index()
        {
            return General();
        }

        #region General Setting
        public ActionResult General()
        {
            var model = new AdminGeneralSettingViewModel
            {
                WebsiteName = SettingsService.GetSetting("WebsiteName"),
                WebsiteUrl = SettingsService.GetSetting("WebsiteUrl"),
                PageTitle = SettingsService.GetSetting("PageTitle"),
                MetaDesc = SettingsService.GetSetting("MetaDesc"),
            };

            return View("General", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult General(AdminGeneralSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting("WebsiteName", setting.WebsiteName);
                        SettingsService.SetSetting("WebsiteUrl", setting.WebsiteUrl);
                        SettingsService.SetSetting("PageTitle", setting.PageTitle);
                        SettingsService.SetSetting("MetaDesc", setting.MetaDesc);


                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }



            return View("General", setting);
        }

        #endregion General Setting

        #region TermsConditions Setting
        public ActionResult TermsConditions()
        {

            return View();
        }
        #endregion TermsConditions Setting

        #region Email Setting
        public ActionResult Email()
        {

            return View();
        }
        #endregion Email Setting

        #region Registration Setting
        public ActionResult Registration()
        {

            return View();
        }
        #endregion Registration Setting

        #region Language Setting
        public ActionResult Language()
        {
            var model = new AdminLanguageSettingViewModel
            {
                LanguageDefault = LocalizationService.DefaultLanguage.Id,
                AllLanguage = LocalizationService.GetBaseSelectListLanguages(LocalizationService.GetAll())
            };

            return View("Language", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Language(AdminLanguageSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    setting.AllLanguage = LocalizationService.GetBaseSelectListLanguages(LocalizationService.GetAll());

                    try
                    {
                        SettingsService.SetSetting("LanguageDefault", setting.LanguageDefault.ToString());


                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }



            return View("Language", setting);
        }
        #endregion

        #region Themes Setting
        public ActionResult Themes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Themes(string activetheme,string atv)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        if(atv == "Deaactive")
                        {
                            SettingsService.SetSetting("Theme", "");
                        }
                        else
                        {
                            SettingsService.SetSetting("Theme", activetheme);
                        }

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }

            return View();
        }

        public ActionResult ThemeConfig(string id)
        {
            ViewBag.id = id;
            ViewBag.json = ThemesSetting.getSettingTheme(id);
            return View();
        }

        [HttpPost]
        [ActionName("ThemeConfig")]
        public ActionResult ThemeConfig1(string id)
        {
            //Request.Form["svd"]
            ViewBag.id = id;
            ViewBag.json = ThemesSetting.getSettingTheme(id);

            foreach (var item in ViewBag.json)
            {
                string buf = Request.Form[(string)item.Name];

                item.Value.Value = buf;
            }

            ThemesSetting.setSettingTheme(id, ViewBag.json);

            return View("ThemeConfig");
        }
        #endregion Themes Setting

        #region CustomCode
        public ActionResult CustomCode()
        {
            var viewModel = new CustomCodeViewModels
            {
                CustomFooterCode = SettingsService.GetSetting("CustomFooterCode"),
                CustomHeaderCode = SettingsService.GetSetting("CustomHeaderCode")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomCode(CustomCodeViewModels setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting("CustomFooterCode", setting.CustomFooterCode);
                        SettingsService.SetSetting("CustomHeaderCode", setting.CustomHeaderCode);


                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }
            
            return View(setting);
        }
        #endregion
    }
}