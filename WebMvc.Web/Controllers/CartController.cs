namespace WebMvc.Web.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.ViewModels;

    public class CartController : BaseController
    {
        public readonly IProductSevice _productSevice;

        public CartController() : base()
        {
            _productSevice = ServiceFactory.Get<IProductSevice>();
        }

        public CartController(IProductSevice productSevice,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _productSevice = productSevice;
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
                            item.Price = p.ToString("N0").Replace(",", ".");
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

                    item.Image = pr.Image;
                    item.link = AppHelpers.ProductUrls(pr.Category_Id, pr.Slug);
                    
                    var price = _productSevice.GetAttributeValue(item.Id, pricmodel.Id);
                    if (price != null)
                    {
                        try
                        {
                            int p = int.Parse(price.Value);
                            item.Priceint = p;
                            item.Price = p.ToString("N0").Replace(",", ".");
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
            var rq = Request.Form;
            var pricmodel = _productSevice.GetAttribute("Price");
            if (viewModel.Products == null) viewModel.Products = new List<CartItemViewModel>();
            
            foreach (var it in viewModel.Products)
            {
                var pr = _productSevice.Get(it.Id);
                if (pr != null)
                {
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