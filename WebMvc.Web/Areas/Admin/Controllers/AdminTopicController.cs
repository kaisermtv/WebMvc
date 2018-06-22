using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Utilities;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;
using WebMvc.Web.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminTopicController : BaseAdminController
    {

        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicServic;
        private readonly IPostSevice _postSevice;

        public AdminTopicController() : base()
        {
            _categoryService = ServiceFactory.Get<ICategoryService>();
            _topicServic = ServiceFactory.Get<ITopicService>();
            _postSevice = ServiceFactory.Get<IPostSevice>();
        }

        public AdminTopicController(IPostSevice postSevice, ITopicService topicService, ICategoryService categoryService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _categoryService = categoryService;
            _topicServic = topicService;
            _postSevice = postSevice;
        }
        // GET: Admin/AdminTopic
        public ActionResult Index()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                TopicListViewModel viewModel = new TopicListViewModel
                {
                    ListTopic = _topicServic.GetList(10, 1)
                };
                return View(viewModel);
            }
        }


        public ActionResult PopupSelect(string seach, string cat, int? p)
        {
            int limit = 10;
            var count = _topicServic.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new TopicListViewModel
            {
                Paging = Paging,
                ListTopic = _topicServic.GetList(limit, Paging.Page)
            };
            return PartialView(viewModel);
        }
        

        #region Create Topic
        //[Authorize]
        public ActionResult Create()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole);
                if (cats.Count > 0)
                {
                    var viewModel = new CreateEditTopicViewModel();
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);



                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));

            }
        }

        [HttpPost]
        //[Authorize]
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
                            topic.Post_Id = post.Id;
                            
                            post.PostContent = viewModel.Content;
                            post.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            post.Topic_Id = topic.Id;
                            post.IsTopicStarter = true;

                            topic.ShotContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(post.PostContent), 50), "....");
                            topic.isAutoShotContent = true;


                            // Sort image out first
                            if (viewModel.Files != null)
                            {
                                // Before we save anything, check the user already has an upload folder and if not create one
                                var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, topic.Id));
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
                                    topic.Image = uploadResult.UploadedFileName;
                                    //viewModel.Image = topic.Image;
                                }

                            }

                            try
                            {
                                _topicServic.Add(topic);
                                _postSevice.Add(post);
                                

                                unitOfWork.Commit();

                                return RedirectToAction("Edit",new { Id = topic.Id });
                            }
                            catch (Exception ex)
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

        #region Edit Topic
        public ActionResult Edit(Guid Id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole);
                if (cats.Count > 0)
                {
                    var viewModel = new CreateEditTopicViewModel();
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);

                    var topic = _topicServic.Get(Id);

                    if(topic == null)
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.NoFindTopic"),
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("Index");
                    }

                    viewModel.Id = topic.Id;
                    viewModel.Name = topic.Name;
                    viewModel.Category = topic.Category_Id;
                    viewModel.IsLocked = topic.IsLocked;
                    viewModel.IsSticky = topic.IsSticky;
                    viewModel.Image = topic.Image;

                    if (topic.Post_Id != null)
                    {
                        var post = _postSevice.Get((Guid)topic.Post_Id);
                        if (post != null)
                        {
                            viewModel.Content = post.PostContent;
                        }
                    }
                    //viewModel.Po = topic.IsLocked;



                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));

            }
        }

        [HttpPost]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateEditTopicViewModel viewModel)
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
                            var topic = _topicServic.Get(viewModel.Id);

                            if (topic == null)
                            {
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Errors.NoFindTopic"),
                                    MessageType = GenericMessages.warning
                                };

                                return RedirectToAction("Index");
                            }


                            viewModel.Image = topic.Image;

                            bool pn = false;
                            Post post;
                            if (topic.Post_Id != null)
                            {
                                post = _postSevice.Get((Guid)topic.Post_Id);
                            } else
                            {
                                post = new Post();
                                pn = true;
                            }

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
                            
                            topic.Name = viewModel.Name;
                            topic.Category_Id = viewModel.Category;
                            topic.Image = viewModel.Image;
                            topic.IsLocked = viewModel.IsLocked;
                            topic.IsSticky = viewModel.IsSticky;
                            topic.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            topic.Post_Id = post.Id;

                            post.PostContent = viewModel.Content;
                            post.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            post.Topic_Id = topic.Id;
                            post.IsTopicStarter = true;

                            topic.ShotContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(post.PostContent), 50), "....");
                            topic.isAutoShotContent = true;
                            try
                            {
                                _topicServic.Update(topic);
                                if(pn) _postSevice.Add(post);
                                else _postSevice.Update(post);


                                unitOfWork.Commit();

                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Success.TopicCreateSuccess"),
                                    MessageType = GenericMessages.success
                                };
                                //return RedirectToAction("Edit", new { Id = topic.Id });
                            }
                            catch (Exception ex)
                            {
                                LoggingService.Error(ex.Message);
                                unitOfWork.Rollback();

                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Error.TopicCreateError"),
                                    MessageType = GenericMessages.warning
                                };
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
        private bool CheckCats(Guid? Id, List<Category> cats)
        {
            bool ret = false;
            foreach (var it in cats)
            {
                if (it.Id == Id)
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