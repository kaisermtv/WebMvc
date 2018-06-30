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
using WebMvc.Utilities;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    public class AdminMenuController : BaseAdminController
    {
        public readonly IMenuService _menuService;
        public readonly ICategoryService _categoryService;
        public readonly ITopicService _topicService;
        public readonly IProductSevice _productSevice;

        public AdminMenuController() : base()
        {
            _menuService = ServiceFactory.Get<IMenuService>();
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _topicService = ServiceFactory.Get<ITopicService>();
            _productSevice = ServiceFactory.Get<IProductSevice>();
        }

        public AdminMenuController(IProductSevice productSevice, ITopicService topicService, ICategoryService categoryService, IMenuService menuService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _menuService = menuService;

            _categoryService = categoryService;
            _topicService = topicService;
            _productSevice = productSevice;
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
                    AllPage = GetPageLink(),
                    AllCat = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll()),
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
                            SortOrder = viewModel.SortOrder
                        };
                        switch (menu.iType)
                        {
                            case 0:
                                menu.Link = viewModel.Link;
                                break;
                            case 1:
                                menu.Link = viewModel.LinkPage;
                                break;
                            case 2:
                                menu.Link = viewModel.LinkCat;
                                break;
                            case 3:
                                menu.Link = viewModel.LinkNews;
                                break;
                            case 4:
                                menu.Link = viewModel.LinkProduct;
                                break;
                        }

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
            viewModel.AllPage = GetPageLink();
            viewModel.AllCat = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll());
            return View(viewModel);
        }

        #endregion


        #region Edit Menu
        private AdminMenuEditViewModel CreateEditMenuViewModel(Menu menu)
        {
            var viewModel = new AdminMenuEditViewModel
            {
                Id = menu.Id,
                ParentMenu = menu.Menu_Id,
                Name = menu.Name,
                Description = menu.Description,
                Colour = menu.Colour,
                iType = menu.iType,
                SortOrder = menu.SortOrder,
                AllMenus = _menuService.GetBaseSelectListMenus(_menuService.GetMenusParenMenu(menu)),
                AllType = GetTypeLink(),
                AllPage = GetPageLink(),
                AllCat = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll()),
            };

            switch (menu.iType)
            {
                case 0:
                    viewModel.Link = menu.Link;
                    break;
                case 1:
                    viewModel.LinkPage = menu.Link;
                    break;
                case 2:
                    viewModel.LinkCat = menu.Link;
                    break;
                case 3:
                    if (!menu.Link.IsNullEmpty())
                    {
                        var a = _topicService.Get(new Guid(menu.Link));
                        if (a != null)
                        {
                            viewModel.LinkNews = menu.Link;
                            viewModel.TitleNews = a.Name;
                        }
                    }
                    break;
                case 4:
                    if (!menu.Link.IsNullEmpty())
                    {
                        var b = _productSevice.Get(new Guid(menu.Link));
                        if (b != null)
                        {
                            viewModel.LinkProduct = menu.Link;
                            viewModel.TitleProduct = b.Name;
                        }
                    }
                     
                    break;
            }

            return viewModel;
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

                        menu.Image = viewModel.Image;
                        menu.Menu_Id = viewModel.ParentMenu;
                        menu.Name = viewModel.Name;
                        menu.Description = viewModel.Description;
                        menu.Colour = viewModel.Colour;
                        menu.iType = viewModel.iType;
                        switch (menu.iType)
                        {
                            case 0:
                                menu.Link = viewModel.Link;
                                break;
                            case 1:
                                menu.Link = viewModel.LinkPage;
                                break;
                            case 2:
                                menu.Link = viewModel.LinkCat;
                                break;
                            case 3:
                                menu.Link = viewModel.LinkNews;
                                break;
                            case 4:
                                menu.Link = viewModel.LinkProduct;
                                break;
                        }
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
            viewModel.AllPage = GetPageLink();
            viewModel.AllCat = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll());
            return View(viewModel);
        }
        #endregion


        #region delete
        public ActionResult Del(Guid id)
        {
            var model = _menuService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Menu không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var submenu = _menuService.GetSubMenus(model);
            if(submenu.Count > 0)
            {

                return View("NotDel",model);
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Del")]
        public ActionResult Del1(Guid id)
        {
            var model = _menuService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Menu không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var submenu = _menuService.GetSubMenus(model);
            if (submenu.Count > 0)
            {

                return View("NotDel", model);
            }


            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _menuService.Del(model);

                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa menu thành công",
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
                        Message = "Có lỗi xảy ra khi xóa menu",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion


        #region Private Function
        private List<SelectListItem> GetTypeLink()
        {
            var lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Liên kết url",Value = "0" });
            lst.Add(new SelectListItem { Text = "Trang có sẵn",Value = "1" });
            lst.Add(new SelectListItem { Text = "Liên kết danh mục",Value = "2" });
            lst.Add(new SelectListItem { Text = "Liên kết bài viết",Value = "3" });
            lst.Add(new SelectListItem { Text = "Liên kết sản phẩm", Value = "4" });


            return lst;
        }

        private List<SelectListItem> GetPageLink()
        {
            var lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Trang chủ", Value = "0" });
            lst.Add(new SelectListItem { Text = "Tin tức", Value = "1" });
            lst.Add(new SelectListItem { Text = "Sản phẩm", Value = "2" });
            lst.Add(new SelectListItem { Text = "Liên hệ", Value = "3" });


            return lst;
        }
        #endregion
    }
}