namespace WebMvc.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Domain.DomainModel.Entities;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.Areas.Admin.ViewModels;
    using WebMvc.Web.ViewModels;

    public class ContactController : BaseController
    {
        public readonly IContactService _contactService;
        public ContactController() : base()
        {
            _contactService = ServiceFactory.Get<IContactService>();
        }

        public ContactController(IContactService contactService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _contactService = contactService;
        }
        // GET: Contact
        public ActionResult Index()
        {
            ContactCreateViewModel contact = new ContactCreateViewModel();

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ContactCreateViewModel contact)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    Contact ct = new Contact();

                    ct.Name = contact.Name;
                    ct.Email = contact.Email;
                    ct.Phone = contact.Phone;
                    ct.Content = contact.Content;

                    try
                    {
                        _contactService.Add(ct);

                        unitOfWork.Commit();

                        return RedirectToAction("Ok");
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);

                        ViewBag.Message = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Contact.Error"),
                            MessageType = GenericMessages.warning
                        };
                    }

                }

            }
            return View(contact);
        }

        public ActionResult Ok()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Ajax(ContactCreateViewModel contact)
        {
            var jsonview = new ContactReturnJson();

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                Contact ct = new Contact();

                ct.Name = contact.Name;
                ct.Email = contact.Email;
                ct.Phone = contact.Phone;
                ct.Content = contact.Content;

                try
                {
                    _contactService.Add(ct);

                    unitOfWork.Commit();

                    jsonview.Status = 1;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);

                    jsonview.Status = 0;
                    jsonview.Message = "";
                }

                return Json(jsonview);
            }
        }
    }
}