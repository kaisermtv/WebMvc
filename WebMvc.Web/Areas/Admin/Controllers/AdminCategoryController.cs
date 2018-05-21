using System;
using System.IO;
using System.Linq;
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
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminCategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController() : base()
        {
            _categoryService = ServiceFactory.Get<ICategoryService>();
        }

        public AdminCategoryController(ICategoryService categoryService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _categoryService = categoryService;
        }

        // GET: Admin/Category
        public ActionResult Index()
        {
            return View();
        }


        [ChildActionOnly]
        public PartialViewResult GetMainCategories()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new ListCategoriesViewModel
                {
                    Categories = _categoryService.GetAll()
                };
                return PartialView(viewModel);
            }
        }

        #region Create
        public ActionResult Create()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var categoryViewModel = new CategoryViewModel
                {
                    AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll())
                };
                return View(categoryViewModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var category = new Category
                        {
                            Name = categoryViewModel.Name,
                            Description = categoryViewModel.Description,
                            IsLocked = categoryViewModel.IsLocked,
                            ModeratePosts = categoryViewModel.ModeratePosts,
                            ModerateTopics = categoryViewModel.ModerateTopics,
                            SortOrder = categoryViewModel.SortOrder,
                            PageTitle = categoryViewModel.PageTitle,
                            MetaDescription = categoryViewModel.MetaDesc,
                            Colour = categoryViewModel.CategoryColour,
                            Category_Id = categoryViewModel.ParentCategory
                        };

                        // Sort image out first
                        if (categoryViewModel.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, category.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = categoryViewModel.Files[0];
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
                                    return View(categoryViewModel);
                                }

                                // Save avatar to user
                                category.Image = uploadResult.UploadedFileName;
                            }

                        }

                        //if (categoryViewModel.ParentCategory != null)
                        //{
                        //    var parentCategory = _categoryService.Get(categoryViewModel.ParentCategory.Value);
                        //    category.ParentCategory = parentCategory;
                        //    SortPath(category, parentCategory);
                        //}

                        _categoryService.Add(category);

                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category Created",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();

                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "There was an error creating the category");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the category");
            }

            categoryViewModel.AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll());
            return View(categoryViewModel);
        }

        #endregion

        #region Edit Category
        private CategoryViewModel CreateEditCategoryViewModel(Category category)
        {
            var categoryViewModel = new CategoryViewModel
            {
                Name = category.Name,
                Description = category.Description,
                IsLocked = category.IsLocked,
                ModeratePosts = category.ModeratePosts == true,
                ModerateTopics = category.ModerateTopics == true,
                SortOrder = category.SortOrder,
                Id = category.Id,
                PageTitle = category.PageTitle,
                MetaDesc = category.MetaDescription,
                Image = category.Image,
                CategoryColour = category.Colour,
                ParentCategory = category.Category_Id,
                AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetCategoriesParenCatregori(category))
            };
            
            return categoryViewModel;
        }

        public ActionResult Edit(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var category = _categoryService.Get(id);
                var categoryViewModel = CreateEditCategoryViewModel(category);

                return View(categoryViewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {

                        var category = _categoryService.Get(categoryViewModel.Id);

                        // Check they are not trying to add a subcategory of this category as the parent or it will break
                        var cats = _categoryService.GetCategoriesParenCatregori(category);
                        var lst = cats.Where(x => x.Id == categoryViewModel.ParentCategory).ToList();
                        if (lst.Count == 0) categoryViewModel.ParentCategory = null;
                        //categoryViewModel.AllCategories = _categoryService.GetBaseSelectListCategories(cats);

                        // Sort image out first
                        if (categoryViewModel.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, categoryViewModel.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = categoryViewModel.Files[0];
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
                                    return View(categoryViewModel);
                                }

                                // Save avatar to user
                                category.Image = uploadResult.UploadedFileName;
                            }

                        }


                        category.Description = categoryViewModel.Description;
                        category.IsLocked = categoryViewModel.IsLocked;
                        category.ModeratePosts = categoryViewModel.ModeratePosts;
                        category.ModerateTopics = categoryViewModel.ModerateTopics;
                        category.Name = categoryViewModel.Name;
                        category.SortOrder = categoryViewModel.SortOrder;
                        category.PageTitle = categoryViewModel.PageTitle;
                        category.MetaDescription = categoryViewModel.MetaDesc;
                        category.Colour = categoryViewModel.CategoryColour;
                        category.Category_Id = categoryViewModel.ParentCategory;
                        

                        _categoryService.Update(category);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category Updated",
                            MessageType = GenericMessages.success
                        };

                        categoryViewModel = CreateEditCategoryViewModel(category);

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category Update Failed",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            return View(categoryViewModel);
        }
        #endregion

    }
}