using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Utilities;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminProductController : BaseAdminController
    {
        private readonly IProductSevice _productSevice;
        private readonly ICategoryService _categoryService;
        private readonly IProductPostSevice _productPostSevice;

        public AdminProductController() : base()
        {
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _productSevice = ServiceFactory.Get<IProductSevice>();
            _productPostSevice = ServiceFactory.Get<IProductPostSevice>();
        }

        public AdminProductController(IProductPostSevice productPostSevice,ICategoryService categoryService, IProductSevice productSevice,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _categoryService = categoryService;
            _productSevice = productSevice;
            _productPostSevice = productPostSevice;
        }

        public ActionResult PopupSelect(string seach, string cat, int? p)
        {
            int limit = 10;
            var count = _productSevice.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new AdminProductViewModel
            {
                Paging = Paging,
                ListProduct = _productSevice.GetList(limit, Paging.Page)
            };
            return PartialView(viewModel);
        }

        #region ProductClass
        public ActionResult Index()
        {
            var model = new AdminProductClassViewModel
            {
                ListProductClass = _productSevice.GetAllProductClass()
            };
            
            return View(model);
        }

        private List<AdminEditProductClassAttributeViewModel> GetProductClassAttribute(ProductClass productClass = null)
        {
            var lst = new List<AdminEditProductClassAttributeViewModel>();

            var attr = _productSevice.GetAllAttribute();

            List<ProductClassAttribute> listcheck = null;
            if (productClass != null)
            {
                listcheck = _productSevice.GetListProductClassAttributeForProductClassId(productClass.Id);
            }


            foreach (var it in attr)
            {
                var a = new AdminEditProductClassAttributeViewModel
                {
                    Id = it.Id,
                    Name = it.LangName,
                };

                if(productClass != null)
                {
                    foreach (var item in listcheck)
                    {
                        if (it.Id == item.ProductAttributeId)
                        {
                            a.IsCheck = true;
                            a.IsShow = item.IsShow;
                            break;
                        }
                    }
                }
                

                lst.Add(a);
            }
            
            return lst;
        }

        public ActionResult Create()
        {
            var model = new AdminEditProductClassViewModel
            {
                AllAttribute = GetProductClassAttribute()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminEditProductClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var productClass = new ProductClass
                        {
                            Name = model.Name,
                            Image = model.Image,
                            Description = model.Description,
                            Colour = model.Colour,
                            IsLocked = model.IsLocked
                        };
                        
                        _productSevice.Add(productClass);

                        if(model.AllAttribute != null)
                        {
                            foreach (var it in model.AllAttribute)
                            {
                                if (it.IsCheck)
                                {
                                    var a = new ProductClassAttribute
                                    {
                                        ProductAttributeId = it.Id,
                                        ProductClassId = productClass.Id,
                                        IsShow = it.IsShow,
                                    };

                                    _productSevice.Add(a);
                                }
                            }
                        }
                            

                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thành công thêm nhóm sản phẩm",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "Lỗi khi thêm nhóm sản phẩm");
                    }
                    
                }
            }

            foreach (var it in model.AllAttribute)
            {
                it.Name = _productSevice.GetAttribute(it.Id).LangName;
            }
            return View(model);
        }

        public ActionResult Edit(Guid Id)
        {
            var productclass = _productSevice.GetProductClass(Id);
            if (productclass == null) return RedirectToAction("index");

            var model = new AdminEditProductClassViewModel
            {
                Id = productclass.Id,
                Colour = productclass.Colour,
                Description = productclass.Description,
                Image = productclass.Image,
                IsLocked = productclass.IsLocked,
                Name = productclass.Name,
                AllAttribute = GetProductClassAttribute(productclass)
            };

            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminEditProductClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var productClass = _productSevice.GetProductClass(model.Id);
                        if (productClass == null) return RedirectToAction("index");

                        model.Image = productClass.Image;

                        productClass.Name = model.Name;
                        productClass.Image = model.Image;
                        productClass.Description = model.Description;
                        productClass.Colour = model.Colour;
                        productClass.IsLocked = model.IsLocked;

                        _productSevice.Update(productClass);

                        _productSevice.DelAllAttributeForProductClass(productClass.Id);
                        if (model.AllAttribute != null)
                        {
                            foreach (var it in model.AllAttribute)
                            {
                                if (it.IsCheck)
                                {
                                    var a = new ProductClassAttribute
                                    {
                                        ProductAttributeId = it.Id,
                                        ProductClassId = productClass.Id,
                                        IsShow = it.IsShow,
                                    };

                                    _productSevice.Add(a);
                                }
                            }
                        }


                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thành công cập nhật nhóm sản phẩm",
                            MessageType = GenericMessages.success
                        };
                        
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);

                        ModelState.AddModelError("", "Lỗi khi thêm nhóm sản phẩm");
                    }

                }
            }

            foreach (var it in model.AllAttribute)
            {
                it.Name = _productSevice.GetAttribute(it.Id).LangName;
            }
            return View(model);
        }

        #endregion

        #region Product
        public ActionResult Product(Guid id)
        {
            var model = new AdminProductViewModel
            {
                ProductClass = id,
                ListProduct = _productSevice.GetList(id,10,1)
            };

            return View(model);
        }

        public ActionResult CreateProduct(Guid g)
        {
            var cats = _categoryService.GetAllowedEditCategories(UsersRole,true);
            
            var model = new AdminEditProductViewModel
            {
                ProductClass = g,
                Categories = _categoryService.GetBaseSelectListCategories(cats),
                AllAttribute = new List<AdminAttributeViewModel>()
            };

            var attr = _productSevice.GetListProductClassAttributeForProductClassId(g);

            foreach (var it in attr)
            {
                var a = _productSevice.GetAttribute(it.ProductAttributeId);
                
                model.AllAttribute.Add(new AdminAttributeViewModel {
                    AttriId = a.Id,
                    Name = a.LangName,
                    ValueType = a.ValueType,
                    IsNull = a.IsNull
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(AdminEditProductViewModel model)
        {
            
            var cats = _categoryService.GetAllowedEditCategories(UsersRole,true);
            model.Categories = _categoryService.GetBaseSelectListCategories(cats);

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        foreach (var it in model.AllAttribute)
                        {
                            var a = _productSevice.GetAttribute(it.AttriId);
                            it.Name = a.LangName;
                            it.ValueType = a.ValueType;
                            it.IsNull = a.IsNull;
                        }

                        var post = new ProductPost();
                        var product = new Product();

                        if (model.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, product.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = model.Files[0];
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
                                    return View(model);
                                }

                                // Save avatar to user
                                model.Image = uploadResult.UploadedFileName;
                            }

                        }

                        post.PostContent = model.Content;
                        post.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                        post.Product_Id = product.Id;
                        post.IsTopicStarter = true;

                        product.Name = model.Name;
                        product.Category_Id = model.Category;
                        product.Image = model.Image;
                        product.IsLocked = model.IsLocked;
                        product.ProductClassId = model.ProductClass;
                        product.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                        product.ProductPost_Id = post.Id;

                        product.ShotContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(post.PostContent), 50), "....");
                        product.isAutoShotContent = true;

                        if (model.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, product.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = model.Files[0];
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
                                    return View(model);
                                }

                                // Save avatar to user
                                product.Image = uploadResult.UploadedFileName;
                            }

                        }

                        _productSevice.Add(product);
                        _productPostSevice.Add(post);
                        
                        foreach (var it in model.AllAttribute)
                        {
                            var val = new ProductAttributeValue
                            {
                                ProductAttributeId = it.AttriId,
                                ProductId = product.Id,
                                Value = it.Value
                            };

                            _productSevice.Add(val);
                        }

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm sản phẩm thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Product",new { id = model.ProductClass });
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError("", "Xảy ra lỗi khi thêm sản phẩm");
                    }
                }
            }
           

            return View(model);
        }

        public ActionResult EditProduct(Guid id)
        {
            var product = _productSevice.Get(id);
            if(product == null) return RedirectToAction("index");
            
            var cats = _categoryService.GetAllowedEditCategories(UsersRole,true);

            var model = new AdminEditProductViewModel
            {
                ProductClass = product.ProductClassId,
                Id = product.Id,
                Category = product.Category_Id,
                Image = product.Image,
                IsLocked = product.IsLocked,
                Name = product.Name,
                Categories = _categoryService.GetBaseSelectListCategories(cats),
                AllAttribute = new List<AdminAttributeViewModel>()
            };


            ProductPost post;
            if (product.ProductPost_Id != null)
            {
                post = _productPostSevice.Get((Guid)product.ProductPost_Id);
                model.Content = post.PostContent;
            }

            var attr = _productSevice.GetListProductClassAttributeForProductClassId(product.ProductClassId);

            foreach (var it in attr)
            {
                var a = _productSevice.GetAttribute(it.ProductAttributeId);

                var m = new AdminAttributeViewModel
                {
                    AttriId = a.Id,
                    Name = a.LangName,
                    ValueType = a.ValueType,
                    IsNull = a.IsNull
                };
                

                var p = _productSevice.GetAttributeValue(product.Id, a.Id);
                if (p != null)
                {
                    m.Id = p.Id;
                    m.Value = p.Value;
                }
                model.AllAttribute.Add(m);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(AdminEditProductViewModel model)
        {

            var cats = _categoryService.GetAllowedEditCategories(UsersRole, true);
            model.Categories = _categoryService.GetBaseSelectListCategories(cats);
            bool getval = false;

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var product = _productSevice.Get(model.Id);
                        if (product == null) return RedirectToAction("index");

                        ProductPost post;
                        if (product.ProductPost_Id != null)
                        {
                            post = _productPostSevice.Get((Guid)product.ProductPost_Id);
                            //model.Content = post.PostContent;
                        }
                        else
                        {
                            post = new ProductPost();
                            post.Product_Id = product.Id;
                            product.ProductPost_Id = post.Id;
                        }

                        model.Image = product.Image;

                        if (model.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, product.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = model.Files[0];
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
                                    return View(model);
                                }

                                // Save avatar to user
                                model.Image = uploadResult.UploadedFileName;
                            }

                        }

                        product.Name = model.Name;
                        product.Category_Id = model.Category;
                        product.Image = model.Image;
                        product.IsLocked = model.IsLocked;
                        product.ProductClassId = model.ProductClass;
                        product.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                        post.PostContent = model.Content;

                        product.ShotContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(post.PostContent), 50), "....");
                        product.isAutoShotContent = true;

                        foreach (var it in model.AllAttribute)
                        {
                            var a = _productSevice.GetAttribute(it.AttriId);
                            it.Name = a.LangName;
                            it.ValueType = a.ValueType;
                            it.IsNull = a.IsNull;

                            _productSevice.Set(product, a, it.Value);
                        }
                        getval = true;


                        _productPostSevice.Update(post);
                        _productSevice.Update(product);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật sản phẩm thành công!",
                            MessageType = GenericMessages.success
                        };

                        //return RedirectToAction("Product", new { id = model.ProductClass });
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError("", "Xảy ra lỗi khi cập nhật sản phẩm");

                        if (!getval)
                        {
                            foreach (var it in model.AllAttribute)
                            {
                                var a = _productSevice.GetAttribute(it.AttriId);
                                it.Name = a.LangName;
                                it.ValueType = a.ValueType;
                                it.IsNull = a.IsNull;
                            }
                        }
                    }
                }
            }


            return View(model);
        }
        #endregion

        #region Attribute
        public ActionResult Attribute()
        {
            var model = new AdminProductAttributeViewModel
            {
                ListProductAttribute = _productSevice.GetAllAttribute()
            };

            return View(model);
        }


        private List<SelectListItem> GetListValueType()
        {
            var lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Text = "text",Value = "0" });
            lst.Add(new SelectListItem { Text = "number", Value = "1" });


            return lst;
        }

        public ActionResult CreateAttribute()
        {
            var model = new AdminCreateProductAttributeViewModel
            {
                AllValueType = GetListValueType(),
                IsNull = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttribute(AdminCreateProductAttributeViewModel model )
        {
            model.AllValueType = GetListValueType();

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var productAttribute = new ProductAttribute
                        {
                            LangName = model.LangName,
                            ValueType = model.ValueType,
                            IsNull = model.IsNull
                        };

                        _productSevice.Add(productAttribute);


                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Product Attribute Created",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Attribute");
                    }
                    catch
                    {
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "There was an error creating the ProductAttribute");
                    }

                }
            }

            return View(model);
        }

        public ActionResult EditAttribute(Guid id)
        {
            var attri = _productSevice.GetAttribute(id);

            if (attri == null) return RedirectToAction("Attribute");

            var model = new AdminCreateProductAttributeViewModel
            {
                AllValueType = GetListValueType(),
                Id = attri.Id,
                LangName = attri.LangName,
                ValueType = attri.ValueType,
                IsNull = attri.IsNull,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAttribute(AdminCreateProductAttributeViewModel model)
        {
            model.AllValueType = GetListValueType();

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var attri = _productSevice.GetAttribute(model.Id);

                        if (attri == null) return RedirectToAction("Attribute");

                        attri.LangName = model.LangName;
                        attri.ValueType = model.ValueType;
                        attri.IsNull = model.IsNull;

                        _productSevice.Update(attri);

                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thuộc tính sản phẩm được cập nhật thành công!",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();
                        
                    }
                    catch
                    {
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin!");
                    }

                }
            }

            return View(model);
        }

        #endregion
    }
}