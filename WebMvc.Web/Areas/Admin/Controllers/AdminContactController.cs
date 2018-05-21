using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminContactController : BaseAdminController
    {
        private readonly IContactService _contactSevice;

        public AdminContactController() : base()
        {
            _contactSevice = ServiceFactory.Get<IContactService>();
        }

        public AdminContactController(IContactService contactSevice, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _contactSevice = contactSevice;
        }

        // GET: Admin/AdminContact
        public ActionResult Index()
        {
            AdminContactViewModel viewModel = new AdminContactViewModel
            {
                ListContact = _contactSevice.GetList(10, 1)
            };
            

            return View(viewModel);
        }

        public ActionResult Edit(Guid Id)
        {
            var contact = _contactSevice.Get(Id);
            if(contact == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Errors.NoFindContact"),
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("Index");
            }


            var viewModel = new AdminContactEditViewModel
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Content = contact.Content,
                IsCheck = contact.IsCheck,
                Note = contact.Note
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminContactEditViewModel viewModel)
        {
            //if (ModelState.IsValid)
            //{
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var contact = _contactSevice.Get(viewModel.Id);
                    if (contact == null)
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.NoFindContact"),
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("Index");
                    }
                    viewModel.Name = contact.Name;
                    viewModel.Email = contact.Email;
                    viewModel.Content = contact.Content;

                    contact.IsCheck = viewModel.IsCheck;
                    contact.Note = viewModel.Note;
                    try
                    {
                        _contactSevice.Update(contact);

                        unitOfWork.Commit();
                    }
                    catch(Exception ex)
                    {
                        LoggingService.Error(ex.Message);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Error.ContactEditError"),
                            MessageType = GenericMessages.warning
                        };
                    }
                }
            //}

            return View(viewModel);
        }
    }
}