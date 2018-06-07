namespace WebMvc.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Domain.Constants;
    using WebMvc.Domain.DomainModel.Entities;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.ViewModels;

    public class ProductController : BaseController
    {
        public readonly IProductSevice _productSevice;
        private readonly ICategoryService _categoryService;
        private readonly IProductPostSevice _productPostSevice;

        public ProductController() : base()
        {
            _productSevice = ServiceFactory.Get<IProductSevice>();
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _productPostSevice = ServiceFactory.Get<IProductPostSevice>();
        }

        public ProductController(IProductPostSevice productPostSevice,ICategoryService categoryService,IProductSevice productSevice,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _productSevice = productSevice;
            _categoryService = categoryService;
            _productPostSevice = productPostSevice;
        }
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowBySlug(string catSlug, string Slug)
        {
            var cat = _categoryService.GetBySlug(catSlug);
            if (cat == null && !cat.IsProduct)
            {
                return RedirectToAction("index", "Catergory");
            }

            var topic = _productSevice.GetBySlug(Slug);
            if (topic == null || cat.Id != topic.Category_Id)
            {
                return RedirectToAction("ShowBySlugProduct", "Category", new { slug = cat.Slug });
            }

            ProductPost post = new ProductPost();

            if (topic.ProductPost_Id != null)
            {
                post = _productPostSevice.Get((Guid)topic.ProductPost_Id);
            }

            var model = new ProductViewModel
            {
                Cat = cat,
                product = topic,
                post = post
            };

            return View(model);
        }
        
        [HttpPost]
        public ActionResult AjaxProductForClass(Guid id, int? page)
        {
            var pcls = _productSevice.GetProductClass(id);
            if (pcls == null) return HttpNotFound();

            int limit = 12;
            var count = _productSevice.GetCount(pcls);
            
            var Paging = CalcPaging(limit, page, count);

            var model = new ClassProductViewModel
            {
                ProductClass = pcls,
                Paging = Paging,
                ListProduct = _productSevice.GetList(pcls, limit, Paging.Page)
            };
            
            return PartialView("AjaxProductForClass",model);
        }
        
    }
}