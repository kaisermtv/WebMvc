namespace WebMvc.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Domain.Constants;
    using WebMvc.Domain.DomainModel.Entities;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.Areas.Admin.ViewModels;
    using WebMvc.Web.ViewModels;

    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicServic;
        private readonly IPostSevice _postSevice;

        public CategoryController() : base()
        {
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _topicServic = ServiceFactory.Get<ITopicService>();
            _postSevice = ServiceFactory.Get<IPostSevice>();
        }

        public CategoryController(IPostSevice postSevice, ITopicService topicService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService, ICategoryService categoryService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _categoryService = categoryService;
            _topicServic = topicService;
            _postSevice = postSevice;
        }
        
        // GET: Category
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ShowBySlug(string Slug,int? p)
        {
            var cat = _categoryService.GetBySlug(Slug);
            if(cat == null)
            {
                return Redirect("/tin-tuc");
            }

            int limit = 12;
            var count = _topicServic.GetCount(cat.Id);

            var Paging = CalcPaging(limit,p, count);
            
            var model = new CategoryTopicListViewModel
            {
                Cat = cat,
                Paging = Paging,
                ListTopic = _topicServic.GetList(cat.Id,limit, Paging.Page)
            };
            
            return View(model);
        }

        public ActionResult Show(Guid Id)
        {
            

            return View();
        }

        public PartialViewResult ListCategorySideMenu()
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
    }
}