namespace WebMvc.Web.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Domain.Constants;
    using WebMvc.Domain.DomainModel.Entities;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.Areas.Admin.ViewModels;
    using WebMvc.Web.ViewModels;

    public class CartController : BaseController
    {
        public readonly IProductSevice _productSevice;
        public readonly IShoppingCartProductService _shoppingCartProductService;
        public readonly IShoppingCartService _shoppingCartService;

        public CartController() : base()
        {
            _productSevice = ServiceFactory.Get<IProductSevice>();
            _shoppingCartProductService = ServiceFactory.Get<IShoppingCartProductService>();
            _shoppingCartService = ServiceFactory.Get<IShoppingCartService>();
        }

        public CartController(IShoppingCartService shoppingCartService, IShoppingCartProductService shoppingCartProductService,IProductSevice productSevice,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _productSevice = productSevice;
            _shoppingCartService = shoppingCartService;
            _shoppingCartProductService = shoppingCartProductService;
        }

        public ActionResult Index()
        {
            var viewModel = new CartListViewModel();
            var list = GetShoppingCart();
            //viewModel.Products = GetSopiingCart();

            var pricmodel = _productSevice.GetAttribute("Price");
            var Guaranteemodel = _productSevice.GetAttribute("Guarantee");

            viewModel.Count = list.Count;
            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                CartItemViewModel item = (CartItemViewModel)it.Value;
                
                var pr = _productSevice.Get(item.Id);
                if(pr != null)
                {
                    viewModel.Products.Add(item);

                    item.Image = pr.Image;
                    item.link = AppHelpers.ProductUrls(pr.Category_Id, pr.Slug);

                    var Guarante = _productSevice.GetAttributeValue(item.Id, Guaranteemodel.Id);
                    if (Guarante != null) item.Guarantee = Guarante.Value;

                    var price = _productSevice.GetAttributeValue(item.Id, pricmodel.Id);
                    if (price != null)
                    {
                        try
                        {
                            int p = int.Parse(price.Value);
                            item.Priceint = p;
                            item.Price = p.ToString("N0").Replace(",", ".") + " VND";
                            viewModel.TotalMoney += p*item.Count;
                        }
                        catch
                        {
                            item.Price = "Liên hệ";
                        }
                    }
                    else
                    {
                        item.Price = "Liên hệ";
                    }

                }
            }

            return View(viewModel);
        }


        public ActionResult Commit()
        {
            var viewModel = new CartViewModel();
            var list = GetShoppingCart();
            //viewModel.Products = GetSopiingCart();

            var pricmodel = _productSevice.GetAttribute("Price");
            
            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                CartItemViewModel item = (CartItemViewModel)it.Value;

                var pr = _productSevice.Get(item.Id);
                if (pr != null)
                {
                    viewModel.Products.Add(item);

                    item.name = pr.Name;
                    item.Image = pr.Image;
                    item.link = AppHelpers.ProductUrls(pr.Category_Id, pr.Slug);
                    
                    var price = _productSevice.GetAttributeValue(item.Id, pricmodel.Id);
                    if (price != null)
                    {
                        try
                        {
                            int p = int.Parse(price.Value);
                            item.Priceint = p;
                            item.Price = p.ToString("N0").Replace(",", ".") + "đ";
                            viewModel.TotalMoney += p * item.Count;
                        }
                        catch
                        {
                            item.Price = "Liên hệ";
                        }
                    }
                    else
                    {
                        item.Price = "Liên hệ";
                    }

                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Commit(CartViewModel viewModel)
        {
            var pricmodel = _productSevice.GetAttribute("Price");
            if (viewModel.Products == null) viewModel.Products = new List<CartItemViewModel>();

            foreach (var it in viewModel.Products)
            {
                var pr = _productSevice.Get(it.Id);
                if (pr != null)
                {
                    it.name = pr.Name;
                    it.Image = pr.Image;
                    it.link = AppHelpers.ProductUrls(pr.Category_Id, pr.Slug);

                    var price = _productSevice.GetAttributeValue(it.Id, pricmodel.Id);
                    if (price != null)
                    {
                        try
                        {
                            int p = int.Parse(price.Value);
                            it.Priceint = p;
                            it.Price = p.ToString("N0").Replace(",", ".");
                            viewModel.TotalMoney += p * it.Count;
                        }
                        catch
                        {
                            it.Price = "Liên hệ";
                        }
                    }
                    else
                    {
                        it.Price = "Liên hệ";
                    }

                }
            }

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var shop = new ShoppingCart
                        {
                            Name = viewModel.Name,
                            Phone = viewModel.Phone,
                            Email = viewModel.Email,
                            Addren = viewModel.Addren,
                            ShipName = viewModel.Ship_Name,
                            ShipAddren = viewModel.Ship_Addren,
                            ShipPhone = viewModel.Ship_Phone,
                            ShipNote = viewModel.Ship_Note,
                            TotalMoney = viewModel.TotalMoney.ToString("N0").Replace(",", "."),
                        };
                        _shoppingCartService.Add(shop);

                        foreach (var it in viewModel.Products)
                        {
                            var cartproduct = new ShoppingCartProduct
                            {
                                CountProduct = (int)it.Count,
                                Price = it.Price,
                                ProductId = it.Id,
                                ShoppingCartId = shop.Id
                            };

                            _shoppingCartProductService.Add(cartproduct);
                        }

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đặt hàng thành công",
                            MessageType = GenericMessages.success
                        };

                        var list = GetShoppingCart();
                        list.Clear();

                        return View("Success");
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.ShoppingCartMessage"));
                    }
                }
            }
            
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Addproduct(Guid Id)
        {
            var viewModel = new CartListViewModel();
            var list = GetShoppingCart();
            //viewModel.Products = GetSopiingCart();

            if (!list.ContainsKey(Id.ToString()))
            {
                var product = _productSevice.Get(Id);

                if (product != null)
                {
                    var a = new CartItemViewModel
                    {
                        name = product.Name,
                        Count = 1,
                        Id = product.Id,
                    };
                    
                    list.Add(a.Id.ToString(), a);
                    viewModel.Count = list.Count;

                    viewModel.State = 1;
                }
                else
                {
                    viewModel.State = 0;
                    viewModel.Message = "Không tìm thấy sản phẩm!";
                }

            }

            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                viewModel.Products.Add((CartItemViewModel)it.Value);
            }


            return Json(viewModel);
        }

        [HttpPost]
        public JsonResult SetListProduct(CartListProductViewModel post)
        {
            var viewModel = new CartListViewModel();
            var list = GetShoppingCart();
            list.Clear();

            foreach (var it in post.Products)
            {
                if (!list.ContainsKey(it.Id.ToString()))
                {
                    var product = _productSevice.Get(it.Id);

                    if (product != null)
                    {
                        var a = new CartItemViewModel
                        {
                            name = product.Name,
                            Count = it.Count,
                            Id = product.Id,
                        };

                        list.Add(a.Id.ToString(), a);
                        viewModel.Count = list.Count;
                    }
                }
            }

            viewModel.State = 1;
            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                viewModel.Products.Add((CartItemViewModel)it.Value);
            }


            return Json(viewModel);
        }

        [HttpPost]
        public JsonResult addVariant(Guid Id,int num)
        {
            var viewModel = new CartListViewModel();
            var list = GetShoppingCart();
            //viewModel.Products = GetSopiingCart();

            if (!list.ContainsKey(Id.ToString()))
            {
                var product = _productSevice.Get(Id);

                if (product != null)
                {
                    var a = new CartItemViewModel
                    {
                        name = product.Name,
                        Count = num,
                        Id = product.Id,
                    };

                    list.Add(a.Id.ToString(), a);
                    viewModel.Count = list.Count;

                    viewModel.State = 1;
                }
                else
                {
                    viewModel.State = 0;
                    viewModel.Message = "Không tìm thấy sản phẩm!";
                }

            }
            else
            {
                ((CartItemViewModel)list[Id.ToString()]).Count = num;
                viewModel.State = 1;
            }

            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                viewModel.Products.Add((CartItemViewModel)it.Value);
            }


            return Json(viewModel);
        }

        [HttpPost]
        public JsonResult removeItem(Guid Id)
        {
            var viewModel = new CartListViewModel();
            var list = GetShoppingCart();
            //viewModel.Products = GetSopiingCart();

            if (list.ContainsKey(Id.ToString()))
            {
                list.Remove(Id.ToString());
                viewModel.Count = list.Count;

                viewModel.State = 1;
            }

            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                viewModel.Products.Add((CartItemViewModel)it.Value);
            }


            return Json(viewModel);
        }

        private Hashtable GetShoppingCart()
        {
            Hashtable list = null;
            try
            {
                list = (Hashtable)Session["SopiingCart"];
            }
            catch {}
            
            if(list == null)
            {
                list = new Hashtable();
                Session["SopiingCart"] = list;
            }
            
            return list;
        }
    }
}