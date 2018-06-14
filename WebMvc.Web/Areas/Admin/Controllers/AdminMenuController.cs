using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    public class AdminMenuController : BaseAdminController
    {
        public readonly IMenuService _menuService;

        public AdminMenuController() : base()
        {
            _menuService = ServiceFactory.Get<IMenuService>();
        }

        public AdminMenuController(IMenuService menuService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _menuService = menuService;
        }

        // GET: Admin/AdminMenu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var viewModel = new AdminMenusViewModel
            {
                Menus = _menuService.GetAll()
            };
            return View(viewModel);
        }


        [ChildActionOnly]
        public PartialViewResult GetMainMenus()
        {
            var viewModel = new AdminMenusViewModel
            {
                Menus = _menuService.GetAll()
            };
            return PartialView(viewModel);
        }


        #region Create
        public ActionResult Create()
        {
                var ViewModel = new AdminMenuEditViewModel
                {
                    AllMenus = _menuService.GetBaseSelectListMenus(_menuService.GetAll()),
                    AllType = GetTypeLink(),
                };
                return View(ViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminMenuEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var menu = new Menu
                        {
                            Menu_Id = viewModel.ParentMenu,
                            Name = viewModel.Name,
                            Description = viewModel.Description,
                            Colour = viewModel.Colour,
                            iType = viewModel.iType,
                            Link = viewModel.Link,
                            SortOrder = viewModel.SortOrder
                        };

                        // Sort image out first
                        if (viewModel.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, menu.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = viewModel.Files[0];
                            if (file != null)
                            {
                                // If successful then upload the file
                                var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, LocalizationService, true);

                                if (!uploadResult.UploadSuccessful)
                                {
                                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                    {
                                        Message = uploadResult.ErrorMessage,
                                        MessageType = GenericMessages.danger
                                    };
                                    return View(viewModel);
                                }

                                // Save avatar to user
                                menu.Image = uploadResult.UploadedFileName;
                            }

                        }

                        //if (categoryViewModel.ParentCategory != null)
                        //{
                        //    var parentCategory = _categoryService.Get(categoryViewModel.ParentCategory.Value);
                        //    category.ParentCategory = parentCategory;
                        //    SortPath(category, parentCategory);
                        //}

                        _menuService.Add(menu);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm menu thành công",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm menu");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the category");
            }

            viewModel.AllMenus = _menuService.GetBaseSelectListMenus(_menuService.GetAll());
            viewModel.AllType = GetTypeLink();
            return View(viewModel);
        }

        #endregion


        #region Edit Category
        private AdminMenuEditViewModel CreateEditMenuViewModel(Menu menu)
        {
            var ViewModel = new AdminMenuEditViewModel
            {
                ParentMenu = menu.Menu_Id,
                Name = menu.Name,
                Description = menu.Description,
                Colour = menu.Colour,
                iType = menu.iType,
                Link = menu.Link,
                SortOrder = menu.SortOrder,
                AllMenus = _menuService.GetBaseSelectListMenus(_menuService.GetMenusParenMenu(menu)),
                AllType = GetTypeLink(),
            };

            return ViewModel;
        }

        public ActionResult Edit(Guid id)
        {
                var menu = _menuService.Get(id);
                var ViewModel = CreateEditMenuViewModel(menu);

                return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminMenuEditViewModel viewModel)
        {
            var menu = _menuService.Get(viewModel.Id);
            if (menu == null) RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Check they are not trying to add a subcategory of this category as the parent or it will break
                        var cats = _menuService.GetMenusParenMenu(menu);
                        var lst = cats.Where(x => x.Id == viewModel.ParentMenu).ToList();
                        if (lst.Count == 0) viewModel.ParentMenu = null;
                        //categoryViewModel.AllCategories = _categoryService.GetBaseSelectListCategories(cats);

                        viewModel.Image = menu.Image;
                        // Sort image out first
                        if (viewModel.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, viewModel.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = viewModel.Files[0];
                            if (file != null)
                            {
                                // If successful then upload the file
                                var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, LocalizationService, true);

                                if (!uploadResult.UploadSuccessful)
                                {
                                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                    {
                                        Message = uploadResult.ErrorMessage,
                                        MessageType = GenericMessages.danger
                                    };
                                    return View(viewModel);
                                }

                                // Save avatar to user
                                viewModel.Image = uploadResult.UploadedFileName;
                            }

                        }


                        menu.Menu_Id = viewModel.ParentMenu;
                        menu.Name = viewModel.Name;
                        menu.Description = viewModel.Description;
                        menu.Colour = viewModel.Colour;
                        menu.iType = viewModel.iType;
                        menu.Link = viewModel.Link;
                        menu.SortOrder = viewModel.SortOrder;

                        _menuService.Update(menu);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật thành công",
                            MessageType = GenericMessages.success
                        };
                        
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi cập nhật menu",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            viewModel.AllMenus = _menuService.GetBaseSelectListMenus(_menuService.GetMenusParenMenu(menu));
            viewModel.AllType = GetTypeLink();
            return View(viewModel);
        }
        #endregion

        #region Private Function
        private List<SelectListItem> GetTypeLink()
        {
            var lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Liên kết url",Value = "0" });
            lst.Add(new SelectListItem { Text = "Trang có sẵn",Value = "1" });
            lst.Add(new SelectListItem { Text = "Liên kết danh mục",Value = "2" });
            lst.Add(new SelectListItem { Text = "Liên kết sản phẩm",Value = "3" });
            lst.Add(new SelectListItem { Text = "Liên kết bài viết",Value = "4" });


            return lst;
        }
        #endregion
    }
}