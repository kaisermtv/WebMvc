namespace WebMvc.Web.Areas.Admin.Controllers
{
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
    using WebMvc.Web.Application;
    using WebMvc.Web.Areas.Admin.ViewModels;

    public class AdminEmployeesController : BaseAdminController
    {
        public readonly IEmployeesRoleService _employeesRoleService;
        public readonly IEmployeesService _employeesService;


        public AdminEmployeesController()
            : base()
        {
            _employeesRoleService = ServiceFactory.Get<IEmployeesRoleService>();
            _employeesService = ServiceFactory.Get<IEmployeesService>();
        }

        public AdminEmployeesController(IEmployeesService employeesService,IEmployeesRoleService employeesRoleService,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _employeesService = employeesService;
            _employeesRoleService = employeesRoleService;
        }

        // GET: Admin/AdminEmployees
        public ActionResult Index()
        {
            var modelView = new AdminEmployeesViewModel
            {
                ListEmployees = _employeesService.GetAll()
            };

            return View(modelView);
        }

        public ActionResult Create()
        {
            var modelView = new CreateEditEmployeesViewModel {
                Roles = _employeesRoleService.GetBaseSelectListEmployeesRole( _employeesRoleService.GetAll())
            };

            return View(modelView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditEmployeesViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var emp = new Employees
                        {
                            Name = modelView.Name,
                            RoleId = modelView.RoleId,
                            Phone = modelView.Phone,
                            Email = modelView.Email,
                            Skype = modelView.Skype,
                        };

                        _employeesService.Add(emp);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm nhân viên thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhân viên!");
                    }
                }
            }
            modelView.Roles = _employeesRoleService.GetBaseSelectListEmployeesRole(_employeesRoleService.GetAll());
            return View(modelView);
        }


        public ActionResult Edit(Guid id)
        {
            var emp = _employeesService.Get(id);
            if (emp == null) return RedirectToAction("ListRole");

            var modelView = new CreateEditEmployeesViewModel
            {
                Id = emp.Id,
                Name = emp.Name,
                RoleId = emp.RoleId,
                Phone = emp.Phone,
                Email = emp.Email,
                Skype = emp.Skype,

                Roles = _employeesRoleService.GetBaseSelectListEmployeesRole(_employeesRoleService.GetAll())
            };

            return View(modelView);
        }

        [HttpPost]
        public ActionResult Edit(CreateEditEmployeesViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var emp = _employeesService.Get(modelView.Id);

                        emp.Name = modelView.Name;
                        emp.RoleId = modelView.RoleId;
                        emp.Phone = modelView.Phone;
                        emp.Email = modelView.Email;
                        emp.Skype = modelView.Skype;

                        _employeesService.Update(emp);
                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật thành công",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi cập nhật thông tin!",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            modelView.Roles = _employeesRoleService.GetBaseSelectListEmployeesRole(_employeesRoleService.GetAll());
            return View(modelView);
        }


        #region delete
        public ActionResult Delete(Guid id)
        {
            var model = _employeesService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Nhân viên không tồn tại",
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
            var model = _employeesService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Nhân viên không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _employeesService.Del(model);


                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa nhân viên thành công",
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
                        Message = "Có lỗi xảy ra khi xóa nhân viên",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion


        #region EmployeesRole

        public ActionResult ListRole()
        {
            var modelView = new AdminEmployeesRoleViewModel
            {
                ListEmployees = _employeesRoleService.GetAll()
            };

            return View(modelView);
        }

        public ActionResult CreateRole()
        {
            var modelView = new CreateEditEmployeesRoleViewModel();

            return View(modelView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(CreateEditEmployeesRoleViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var emp = new EmployeesRole
                        {
                            Name = modelView.Name,
                            Description = modelView.Description
                        };

                        _employeesRoleService.Add(emp);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm chức vụ nhân viên thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("ListRole");
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm chức vụ!");
                    }
                }
            }

            return View(modelView);
        }
        
        public ActionResult EditRole(Guid id)
        {
            var role = _employeesRoleService.Get(id);
            if(role == null) return RedirectToAction("ListRole");

            var modelView = new CreateEditEmployeesRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                SortOrder = role.SortOrder
            };
            
            return View(modelView);
        }

        [HttpPost]
        public ActionResult EditRole(CreateEditEmployeesRoleViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var role = _employeesRoleService.Get(modelView.Id);

                        role.Name = modelView.Name;
                        role.Description = modelView.Description;
                        role.SortOrder = modelView.SortOrder;

                        _employeesRoleService.Update(role);
                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật thành công",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi cập nhật thông tin!",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            return View(modelView);
        }


        #region delete
        public ActionResult DelRole(Guid id)
        {
            var model = _employeesRoleService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Nhóm nhân viên không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var nhanvien = _employeesService.GetList(model);
            if (nhanvien.Count > 0)
            {
                return View("NotDel", model);
            }


            return View(model);
        }

        [HttpPost]
        [ActionName("DelRole")]
        public ActionResult DelRole1(Guid id)
        {
            var model = _employeesRoleService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Nhóm nhân viên không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var nhanvien = _employeesService.GetList(model);
            if (nhanvien.Count > 0)
            {
                return View("NotDel", model);
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _employeesRoleService.Del(model);


                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa nhóm nhân viên thành công",
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
                        Message = "Có lỗi xảy ra khi xóa nhóm nhân viên",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion

        #endregion
    }
}