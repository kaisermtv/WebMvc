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

    public class TopicController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicServic;
        private readonly IPostSevice _postSevice;

        public TopicController() : base()
        {
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _topicServic = ServiceFactory.Get<ITopicService>();
            _postSevice = ServiceFactory.Get<IPostSevice>();
        }

        public TopicController(IPostSevice postSevice,ITopicService topicService,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService,ICategoryService categoryService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _categoryService = categoryService;
            _topicServic = topicService;
            _postSevice = postSevice;
        }

        

        #region List Topic
        public ActionResult Index()
        {
            TopicListViewModel viewModel = new TopicListViewModel
            {
                ListTopic = _topicServic.GetList(10, 1)
            };
            return View(viewModel);
        }

        public ActionResult List(Guid id)
        {
            return View();
        }

        public ActionResult PopupSelectList(string inputid, string inputname ,int? p)
        {
            int limit = 12;
            var count = _topicServic.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var model = new CategoryTopicListViewModel
            {
                Paging = Paging,
                ListTopic = _topicServic.GetList(limit, Paging.Page)
            };

            return View(model);
        }
        #endregion

        #region Show
        public ActionResult ShowBySlug(string catSlug, string Slug)
        {
            var cat = _categoryService.GetBySlug(catSlug);
            if (cat == null)
            {
                return RedirectToAction("index","Catergory");
            }

            var topic = _topicServic.GetBySlug(Slug);
            if (topic == null || cat.Id != topic.Category_Id)
            {
                return RedirectToAction("ShowBySlug", "Category", new { slug = cat.Slug });
            }

            Post post = new Post();

            if (topic.Post_Id != null)
            {
                post = _postSevice.Get((Guid)topic.Post_Id);
            }

            var model = new TopicViewModel
            {
                Cat = cat,
                topic = topic,
                post = post
            };

            return View(model);
        }
        public ActionResult Show(Guid Id)
        {
            //_topicServic.get

            return View();
        }
        #endregion

        #region Create Topic
        [Authorize]
        public ActionResult Create()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole);
                if(cats.Count > 0)
                {
                    var viewModel = new CreateEditTopicViewModel();
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);



                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditTopicViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole);
                if (cats.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (CheckCats(viewModel.Category, cats))
                        {
                            var topic = new Topic();
                            var post = new Post();

                            topic.Name = viewModel.Name;
                            topic.Category_Id = viewModel.Category;
                            topic.IsLocked = viewModel.IsLocked;
                            topic.IsSticky = viewModel.IsSticky;
                            topic.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            topic.Id = post.Id;

                            post.PostContent = viewModel.Content;
                            post.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            post.Topic_Id = topic.Id;
                            post.IsTopicStarter = true;



                            try
                            {
                                _topicServic.Add(topic);
                                _postSevice.Add(post);








                                unitOfWork.Commit();

                            }
                            catch(Exception ex)
                            {
                                LoggingService.Error(ex.Message);
                                unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            //viewModel.Category = null;
                            //No permission to create a Poll so show a message but create the topic
                            //TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            //{
                            //    Message = LocalizationService.GetResourceString("Errors.NoPermissionCatergory"),
                            //    MessageType = GenericMessages.info
                            //};
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.CatergoryMessage"));
                        }

                    }
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);
                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        #endregion

        #region Function 
        private bool CheckCats(Guid? Id,List<Category> cats)
        {
            bool ret = false;
            foreach(var it in cats)
            {
                if(it.Id == Id)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
        #endregion
    }
}