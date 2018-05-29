using Microsoft.Practices.Unity;
using System.Web.Mvc;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.IOC.Quartz;
using WebMvc.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Services.Data.UnitOfWork;

namespace WebMvc.IOC
{
    /// <summary>
    /// Bind the given interface in request scope
    /// </summary>
    public static class IocExtensions
    {
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }

    }

    /// <summary>
    /// The injection for Unity
    /// </summary>
    public static partial class UnityHelper
    {

        public static IUnityContainer Start()
        {
            var container = new UnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            var buildUnity = BuildUnityContainer(container);
            return buildUnity;
        }

        /// <summary>
        /// Inject
        /// </summary>
        /// <returns></returns>
        private static IUnityContainer BuildUnityContainer(UnityContainer container)
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.BindInRequestScope<IWebMvcContext, WebMvcContext>();
            container.BindInRequestScope<IUnitOfWorkManager, UnitOfWorkManager>();
           
            // Quartz
            container.AddNewExtension<QuartzUnityExtension>();

            //Bind the various domain model services and repositories that e.g. our controllers require         
            container.BindInRequestScope<ILoggingService, LoggingService>();
            container.BindInRequestScope<ICacheService, CacheService>();
            container.BindInRequestScope<IMembershipService, MembershipService>();
            container.BindInRequestScope<ISettingsService, SettingsService>();
            container.BindInRequestScope<IConfigService, ConfigService>();
            container.BindInRequestScope<ILocalizationService, LocalizationService>(); 
            container.BindInRequestScope<ICategoryService, CategoryService>(); 
            container.BindInRequestScope<ITopicService, TopicService>();
            container.BindInRequestScope<IPostSevice, PostSevice>(); 
            container.BindInRequestScope<IProductPostSevice, ProductPostSevice>();
            container.BindInRequestScope<IRoleSevice, RoleSevice>(); 
            container.BindInRequestScope<IPermissionService, PermissionService>(); 
            container.BindInRequestScope<IContactService, ContactService>(); 
            container.BindInRequestScope<IBookingSevice, BookingSevice>();
            container.BindInRequestScope<ITypeRoomSevice, TypeRoomSevice>(); 
            container.BindInRequestScope<IProductSevice, ProductSevice>();


            CustomBindings(container);

            return container;
        }

        static partial void CustomBindings(UnityContainer container);
    }

    // Example of adding your own bindings, just create a partial class and implement
    // the CustomBindings method and add your bindings as shown below
    //public static partial class UnityHelper
    //{
    //    static partial void CustomBindings(UnityContainer container)
    //    {
    //        container.BindInRequestScope<IBlockRepository, BlockRepository>();
    //        container.BindInRequestScope<IBlockService, BlockService>();
    //    }
    //}
}