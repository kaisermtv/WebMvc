using System;
using System.Collections.Generic;
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
    public class AdminProductController : BaseAdminController
    {
        private readonly IProductSevice _productSevice;
        private readonly ICategoryService _categoryService;

        public AdminProductController() : base()
        {
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _productSevice = ServiceFactory.Get<IProductSevice>();
        }

        public AdminProductController(ICategoryService categoryService, IProductSevice productSevice,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _categoryService = categoryService;
            _productSevice = productSevice;
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

        private List<AdminEditProductClassAttributeViewModel> GetProductClassAttribute()
        {
            var lst = new List<AdminEditProductClassAttributeViewModel>();

            var attr = _productSevice.GetAllAttribute();

            foreach (var it in attr)
            {
                var a = new AdminEditProductClassAttributeViewModel
                {
                    Id = it.Id,
                    Name = it.LangName,
                };

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
            var cats = _categoryService.GetAllowedEditCategories(UsersRole);
            
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
            
            var cats = _categoryService.GetAllowedEditCategories(UsersRole);
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

                        var product = new Product
                        {
                            Name = model.Name,
                            Category_Id = model.Category,
                            Image = model.Image,
                            IsLocked = model.IsLocked,
                            ProductClassId = model.ProductClass,
                            MembershipUser_Id = LoggedOnReadOnlyUser.Id
                        };
                        
                        _productSevice.Add(product);

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
            
            var cats = _categoryService.GetAllowedEditCategories(UsersRole);

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

            var cats = _categoryService.GetAllowedEditCategories(UsersRole);
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

                        var product = new Product
                        {
                            Name = model.Name,
                            Category_Id = model.Category,
                            Image = model.Image,
                            IsLocked = model.IsLocked,
                            ProductClassId = model.ProductClass,
                            MembershipUser_Id = LoggedOnReadOnlyUser.Id
                        };

                        //_productSevice.Update(product);

                        foreach (var it in model.AllAttribute)
                        {
                            var val = new ProductAttributeValue
                            {
                                Id = it.Id,
                                ProductAttributeId = it.AttriId,
                                ProductId = product.Id,
                                Value = it.Value
                            };

                            //_productSevice.Set(val);
                        }

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm sản phẩm thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Product", new { id = model.ProductClass });
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError("", "Xảy ra lỗi khi thêm sản phẩm");
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
                AllValueType = GetListValueType()
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
                            Message = "Product Attribute Created",
                            MessageType = GenericMessages.success
                        };
                        unitOfWork.Commit();
                        
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

        #endregion
    }
}