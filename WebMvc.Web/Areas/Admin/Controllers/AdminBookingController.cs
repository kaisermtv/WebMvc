using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminBookingController : BaseAdminController
    {
        private readonly IBookingSevice _bookingSevice;
        private readonly ITypeRoomSevice _typeRoomSevice;

        public AdminBookingController() : base()
        {
            _bookingSevice = ServiceFactory.Get<IBookingSevice>();
            _typeRoomSevice = ServiceFactory.Get<ITypeRoomSevice>();
        }

        public AdminBookingController(ITypeRoomSevice typeRoomSevice,IBookingSevice bookingSevice, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _bookingSevice = bookingSevice;
            _typeRoomSevice = typeRoomSevice;
        }


        // GET: Admin/AdminBooking
        public ActionResult Index()
        {
            AdminBookingListViewModel viewModel = new AdminBookingListViewModel
            {
                ListBooking = _bookingSevice.GetList(10,1)
            };

            return View(viewModel);
        }

        #region delete
        public ActionResult Delete(Guid id)
        {
            var model = _bookingSevice.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Đơn đặt phòng không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete1(Guid id)
        {
            var model = _bookingSevice.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Đơn đặt phòng không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _bookingSevice.Del(model);


                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa đơn đặt phòng thành công",
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("index");
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    unitOfWork.Rollback();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Có lỗi xảy ra khi xóa đơn đặt phòng",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion

        #region TypeRoom Action
        public ActionResult TypeRoom()
        {
            AdminTypeRoomListViewModel viewModel = new AdminTypeRoomListViewModel
            {
                ListTypeRoom = _typeRoomSevice.GetAll()
            };

            return View(viewModel);
        }

        public ActionResult NewTypeRoom()
        {
            var viewModel = new AdminTypeRoomEditViewModel
            {
                IsShow = true
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewTypeRoom(AdminTypeRoomEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var typeRoom = new TypeRoom
                        {
                            Name = viewModel.Name,
                            IsShow = viewModel.IsShow,
                            Note = viewModel.Note
                        };

                        _typeRoomSevice.Add(typeRoom);

                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Type Room Created",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();

                        return RedirectToAction("TypeRoom");
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "There was an error creating the Type Room");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the Type Room");
            }
            
            return View(viewModel);
        }

        public ActionResult EditTypeRoom(Guid Id)
        {
            var typeRoom = _typeRoomSevice.Get(Id);
            if (typeRoom == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Errors.NoFindTypeRoom"),
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("TypeRoom");
            }

            var viewModel = new AdminTypeRoomEditViewModel
            {
                Id = typeRoom.Id,
                Name = typeRoom.Name,
                IsShow = typeRoom.IsShow,
                Note = typeRoom.Note
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTypeRoom(AdminTypeRoomEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var typeRoom = _typeRoomSevice.Get(viewModel.Id);
                    if (typeRoom == null)
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.NoFindTypeRoom"),
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("TypeRoom");
                    }

                    typeRoom.Name = viewModel.Name;
                    typeRoom.IsShow = viewModel.IsShow;
                    typeRoom.Note = viewModel.Note;
                    try
                    {
                        _typeRoomSevice.Update(typeRoom);

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex.Message);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Error.TypeRoomEditError"),
                            MessageType = GenericMessages.warning
                        };
                    }
                }
            }

            return View(viewModel);
        }

        #endregion
    }
}