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

    public class BookingController : BaseController
    {
        public readonly IBookingSevice _bookingSevice;
        public readonly ITypeRoomSevice _typeRoomSevice;

        public BookingController() : base()
        {
            _bookingSevice = ServiceFactory.Get<IBookingSevice>();
            _typeRoomSevice = ServiceFactory.Get<ITypeRoomSevice>();
        }

        public BookingController(ITypeRoomSevice typeRoomSevice,IBookingSevice bookingSevice,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _bookingSevice = bookingSevice;
            _typeRoomSevice = typeRoomSevice;
        }

        // GET: Booking
        public ActionResult Index()
        {
            BookingCreateViewModel viewModel = new BookingCreateViewModel();

            viewModel.CheckIn = DateTime.Now;

            viewModel.CheckOut = DateTime.Now.AddDays(1);

            viewModel.ListTypeRoom = _typeRoomSevice.GetBaseSelectListTypeRooms(_typeRoomSevice.GetList(true));

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BookingCreateViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    Booking bk = new Booking();

                    bk.NamePartner = modelView.NamePartner;
                    bk.Email = modelView.Email;
                    bk.CheckIn = modelView.CheckIn;
                    bk.CheckOut = modelView.CheckOut;
                    bk.Adukts = modelView.Adukts;
                    bk.Adolescent = modelView.Adolescent;
                    bk.Children = modelView.Children;
                    bk.Phone = modelView.Phone;
                    bk.TypeRoom_Id = modelView.TypeRoom_Id;

                    try
                    {
                        _bookingSevice.Add(bk);

                        unitOfWork.Commit();

                        return RedirectToAction("Ok");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);

                        ViewBag.Message = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Booking.Error"),
                            MessageType = GenericMessages.warning
                        };
                    }

                }

            }

            modelView.ListTypeRoom = _typeRoomSevice.GetBaseSelectListTypeRooms(_typeRoomSevice.GetList(true));
            return View(modelView);
        }

        public ActionResult Ok()
        {
            return View();
        }
    }
}