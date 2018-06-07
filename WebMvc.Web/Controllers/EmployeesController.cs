namespace WebMvc.Web.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Domain.Interfaces.UnitOfWork;
    using WebMvc.Web.Application;
    using WebMvc.Web.ViewModels;

    public class EmployeesController : BaseController
    {
        public readonly IEmployeesRoleService _employeesRoleService;
        public readonly IEmployeesService _employeesService;
        
        public EmployeesController()
            : base()
        {
            _employeesRoleService = ServiceFactory.Get<IEmployeesRoleService>();
            _employeesService = ServiceFactory.Get<IEmployeesService>();
        }

        // GET: Employees
        public EmployeesController(IEmployeesService employeesService, IEmployeesRoleService employeesRoleService,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ICacheService cacheService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _employeesService = employeesService;
            _employeesRoleService = employeesRoleService;
        }

        [ChildActionOnly]
        public ActionResult OnlineOrder()
        {
            var modelView = new EmployeesViewModel
            {
                employeesRoles = _employeesRoleService.GetAll(),
                employees = _employeesService.GetAll(),
            };

            return PartialView(modelView);
        }


    }
}