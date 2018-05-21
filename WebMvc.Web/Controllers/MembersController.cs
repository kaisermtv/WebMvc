namespace WebMvc.Web.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.ViewModels;
    using Application;
    using WebMvc.Domain.DomainModel.Entities;
    using System;

    public partial class MembersController : BaseController
    {
        public MembersController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService,ILocalizationService localizationService)
            :base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        // GET: Members
        [Authorize]
        public ActionResult Index()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var model = MembershipService.GetUser(User.Identity.Name);



                return View(model);
            }
        }

        #region Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (SettingsService.GetSetting("SuspendRegistration") != "true")
            {
                // Populate empty viewmodel
                var viewModel = new MemberAddViewModel();

                // See if a return url is present or not and add it
                var returnUrl = Request["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    viewModel.ReturnUrl = returnUrl;
                }

                return View(viewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(MemberAddViewModel userModel)
        {
            if (SettingsService.GetSetting("SuspendRegistration") != "true" && SettingsService.GetSetting("DisableStandardRegistration") != "true")
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    if (userModel.Password != userModel.ConfirmPassword)
                    {
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Error.PasswordNotConfirm"));
                        return View(userModel);
                    }

                    // First see if there is a spam question and if so, the answer matches
                    if (!string.IsNullOrEmpty(SettingsService.GetSetting("SpamQuestion")))
                    {
                        // There is a spam question, if answer is wrong return with error
                        if (userModel.SpamAnswer == null || userModel.SpamAnswer.Trim() != SettingsService.GetSetting("SpamAnswer"))
                        {
                            // POTENTIAL SPAMMER!
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Error.WrongAnswerRegistration"));
                            return View(userModel);
                        }
                    }

                    // Secondly see if the email is banned
                    //if (_bannedEmailService.EmailIsBanned(userModel.Email))
                    //{
                    //    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Error.EmailIsBanned"));
                    //    return View();
                    //}

                    MembershipUser newuser = new MembershipUser();
                    newuser.UserName = userModel.UserName;
                    newuser.Password = userModel.Password;
                    newuser.Email = userModel.Email;


                    var createStatus = MembershipService.NewUser(newuser);
                    if (createStatus != MembershipCreateStatus.Success)
                    {
                        ModelState.AddModelError(string.Empty, MembershipService.ErrorCodeToString(createStatus));
                        return View(userModel);
                    }
                    else
                    {

                    }

                    try
                    {
                        unitOfWork.Commit();

                        if (Url.IsLocalUrl(userModel.ReturnUrl) && userModel.ReturnUrl.Length > 1 && userModel.ReturnUrl.StartsWith("/")
                            && !userModel.ReturnUrl.StartsWith("//") && !userModel.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(userModel.ReturnUrl);
                        }
                        return RedirectToAction("Index", "Home", new { area = string.Empty });
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        //System.Web.Security.FormsAuthentication.SignOut();
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }


                }
            }
            return View(userModel);
        }
        #endregion


        #region Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            LogOnViewModel viewModel = new LogOnViewModel();

            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LogOnViewModel model)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var username = model.UserName;
                var password = model.Password;

                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = MembershipService.ValidateUser(username, password, System.Web.Security.Membership.MaxInvalidPasswordAttempts);
                        if (user != null)
                        {
                            System.Web.Security.FormsAuthentication.SetAuthCookie(username, model.RememberMe);

                            if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 && model.ReturnUrl.StartsWith("/")
                                        && !model.ReturnUrl.StartsWith("//") && !model.ReturnUrl.StartsWith("/\\"))
                            {
                                return Redirect(model.ReturnUrl);
                            }


                            return RedirectToAction("Index", "Home", new { area = string.Empty });
                        }
                        else
                        {
                            // get here Login failed, check the login status
                            var loginStatus = MembershipService.LastLoginStatus;

                            switch (loginStatus)
                            {
                                case LoginAttemptStatus.UserNotFound:
                                case LoginAttemptStatus.PasswordIncorrect:
                                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Members.Errors.PasswordIncorrect"));
                                    break;

                                case LoginAttemptStatus.PasswordAttemptsExceeded:
                                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Members.Errors.PasswordAttemptsExceeded"));
                                    break;

                                case LoginAttemptStatus.UserLockedOut:
                                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Members.Errors.UserLockedOut"));
                                    break;

                                case LoginAttemptStatus.Banned:
                                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Members.NowBanned"));
                                    break;

                                case LoginAttemptStatus.UserNotApproved:
                                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Members.Errors.UserNotApproved"));
                                    //user = MembershipService.GetUser(username);
                                    //SendEmailConfirmationEmail(user);
                                    break;

                                default:
                                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Members.Errors.LogonGeneric"));
                                    break;
                            }
                        }
                    }
                }
                //catch
                //{
                //    LoggingService.Error(ex);
                //}
                finally
                {
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }

                }

                return View(model);
            }
        }


        [AllowAnonymous]
        public ActionResult LogOut()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                System.Web.Security.FormsAuthentication.SignOut();
                //ViewBag.Message = new GenericMessageViewModel
                //{
                //    Message = LocalizationService.GetResourceString("Members.NowLoggedOut"),
                //    MessageType = GenericMessages.success
                //};
                return RedirectToAction("Index", "Home", new { area = string.Empty });
            }
        }

        #endregion



    }
}