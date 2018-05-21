using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Utilities;
using WebMvc.Web.Areas.Admin.ViewModels;
using static WebMvc.Web.Areas.Admin.ViewModels.ListLanguagesViewModel;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminLanguageController : BaseAdminController
    {
        public AdminLanguageController( ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ImportExport()
        {
            return View();
        }

        /// <summary>
        /// Manage resource keys (for all languages)
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageResourceKeys(int? p, string search)
        {
            try
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    if (!ModelState.IsValid)
                    {
                        var errors = (from key in ModelState.Keys select ModelState[key] into state where state.Errors.Any() select state.Errors.First().ErrorMessage).ToList();
                        ShowErrors(errors);
                    }

                    return ListResourceKeys(p, search);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Get - create a new language
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult CreateLanguage()
        {
            return PartialView();
        }

        /// <summary>
        /// Post - create a new language
        /// </summary>
        /// <param name="languageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult CreateLanguage(CreateLanguageViewModel languageViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get the culture info
                    var cultureInfo = LanguageUtils.GetCulture(languageViewModel.Name);

                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        try
                        {
                            LocalizationService.Add(cultureInfo);
                            unitOfWork.Commit();
                            ShowSuccess("Language Created");
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            throw;
                        }
                    }
                }
                else
                {

                    var errors = (from key in ModelState.Keys select ModelState[key] into state where state.Errors.Any() select state.Errors.First().ErrorMessage).ToList();
                    ShowErrors(errors);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            // Default ie error
            return RedirectToAction("Index");
        }

        public ActionResult AddResourceKey()
        {
            var viewModel = new LocaleResourceKeyViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddResourceKey(LocaleResourceKeyViewModel newResourceKeyViewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var resourceKeyToSave = new LocaleResourceKey
                    {
                        Name = newResourceKeyViewModel.Name,
                        Notes = newResourceKeyViewModel.Notes
                    };

                    LocalizationService.Add(resourceKeyToSave);
                    unitOfWork.Commit();
                    ShowSuccess("Resource key created successfully");
                    var currentLanguage = SettingsService.GetSetting("DefaultLanguage");
                    return RedirectToAction("ManageLanguageResourceValues", new { languageId = currentLanguage });
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    ShowError(ex.Message);
                    LoggingService.Error(ex);
                    return RedirectToAction("AddResourceKey");
                }
            }
        }

        [HttpPost]
        public void UpdateResourceValue(AjaxEditLanguageValueViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        LocalizationService.UpdateResourceString(viewModel.LanguageId, viewModel.ResourceKey,
                                                                 viewModel.NewValue);
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a partial view listing all languages
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult GetLanguages()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new ListLanguagesViewModel { Languages = new List<LanguageDisplayViewModel>() };

                try
                {
                    foreach (var language in LocalizationService.GetAll())
                    {
                        var languageViewModel = new LanguageDisplayViewModel
                        {
                            Id = language.Id,
                            IsDefault = language.Id == LocalizationService.CurrentLanguage.Id,
                            Name = language.Name,
                            LanguageCulture = language.LanguageCulture,
                        };

                        viewModel.Languages.Add(languageViewModel);

                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    LoggingService.Error(ex);
                }

                return PartialView(viewModel);
            }
        }

        public ActionResult ManageLanguageResourceValues(Guid languageId, int? p, string search)
        {
            return GetLanguageResources(false, languageId, p, search);
        }

        #region Private
        private ActionResult GetLanguageResources(bool searchByKey, Guid languageId, int? p, string search)
        {
            try
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var language = LocalizationService.Get(languageId);

                    int count = LocalizationService.GetCountResourceKey();
                    int limit = 30;
                    int MaxPage = count / limit;
                    if (count % limit > 0) MaxPage++;
                    if (MaxPage == 0) MaxPage = 1;

                    if (p == null) p = 1;
                    if (p > MaxPage) p = MaxPage;

                    var resources = LocalizationService.GetListResourceKey((int)p, limit);

                    var resourceListModel = new LanguageListResourcesViewModel
                    {
                        LanguageId = language.Id,
                        LanguageName = language.Name,
                        LocaleResources = new List<LocaleResourceViewModel>(),
                        PageIndex = p,
                        TotalCount = count,
                        Search = search,
                        TotalPages = MaxPage
                    };

                    foreach (var it in resources)
                    {
                        var ResourceString = LocalizationService.GetValueResource(it.Id, languageId);

                        if (ResourceString == null) ResourceString = new LocaleStringResource {
                            ResourceValue = ""
                        };

                        resourceListModel.LocaleResources.Add(new LocaleResourceViewModel {
                            Id = ResourceString.Id,
                            ResourceKeyId = it.Id,
                            LocaleResourceKey = it.Name,
                            ResourceValue = ResourceString.ResourceValue
                        });
                    }



                    return View("ListValues", resourceListModel);

                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            // Default ie error
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create a message to be displayed as an error
        /// </summary>
        /// <param name="message"></param>
        private void ShowError(string message)
        {
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = message,
                MessageType = GenericMessages.danger
            };
        }

        /// <summary>
        /// Create a message to be displayed when some action is successful
        /// </summary>
        /// <param name="message"></param>
        private void ShowSuccess(string message)
        {
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = message,
                MessageType = GenericMessages.success
            };
        }

        /// <summary>
        /// Create a message to be displayed as an error from 
        /// a set of messages
        /// </summary>
        /// <param name="messages"></param>
        private void ShowErrors(IEnumerable<string> messages)
        {
            var errors = new StringBuilder();

            foreach (var message in messages)
            {
                errors.AppendLine(message);
            }

            ShowError(errors.ToString());
        }

        /// <summary>
        /// List out resource keys and allow editing
        /// </summary>
        /// <returns></returns>
        private ActionResult ListResourceKeys(int? page, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                int count = LocalizationService.GetCountResourceKey();
                int limit = 30;
                int MaxPage = count / limit;
                if (count % limit > 0) MaxPage++;
                if (MaxPage == 0) MaxPage = 1;

                if (page == null) page = 1;
                if (page > MaxPage) page = MaxPage;

                var resources = LocalizationService.GetListResourceKey((int)page, limit);

                var resourceListModel = new ResourceKeyListViewModel
                {
                    ResourceKeys = new List<LocaleResourceKeyViewModel>(),
                    PageIndex = page,
                    TotalCount = count,
                    Search = search,
                    TotalPages = MaxPage
                };

                foreach (var resource in resources)
                {
                    resourceListModel.ResourceKeys.Add(new LocaleResourceKeyViewModel
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Notes = resource.Notes,
                        DateAdded = resource.DateAdded
                    });
                }
                
                return View("ListKeys", resourceListModel);
            }
        }
        #endregion

    }
}