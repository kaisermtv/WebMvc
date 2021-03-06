﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.General;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Utilities;
using WebMvc.Web.Application.StorageProviders;
using WebMvc.Web.ViewModels;

namespace WebMvc.Web.Application
{
    public class AppHelpers
    {
        #region Application

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public static bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".htm":
                case ".html":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                    return true;
            }

            return false;
        }

        #endregion

        #region Themes

        /// <summary>
        /// Gets the theme folders currently installed
        /// </summary>
        /// <returns></returns>
        public static List<string> GetThemeFolders()
        {
            var folders = new List<string>();
            var themeRootFolder = HostingEnvironment.MapPath($"~/{SiteConstants.Instance.ThemeRootFolderName}");
            if (Directory.Exists(themeRootFolder))
            {
                folders.AddRange(Directory.GetDirectories(themeRootFolder)
                                .Select(Path.GetFileName)
                                .Where(x => !x.ToLower().Contains("base")));
            }
            else
            {
                throw new ApplicationException("Theme folder not found");
            }
            return folders;
        }


        #endregion

        #region SEO

        private const string CanonicalNext = "<link href=\"{0}\" rel=\"next\" />";
        private const string CanonicalPrev = "<link href=\"{0}\" rel=\"prev\" />";
        private const string Canonical = "<link href=\"{0}\" rel=\"canonical\" />";

        public static string CanonicalPagingTag(int totalItemCount, int pageSize, HtmlHelper helper)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var currentAction = helper.ViewContext.RouteData.GetRequiredString("Action");
            var url = urlHelper.Action(currentAction, new { });

            var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);

            var nextTag = String.Empty;
            var previousTag = String.Empty;

            var req = HttpContext.Current.Request["p"];
            var page = req != null ? Convert.ToInt32(req) : 1;

            // Sort the canonical tag out
            var canonicalTag = String.Format(Canonical, page <= 1 ? url : String.Format(AppConstants.PagingUrlFormat, url, page));

            // On the first page       
            if (pageCount > 1 & page <= 1)
            {
                nextTag = String.Format(CanonicalNext, String.Format(AppConstants.PagingUrlFormat, url, (page + 1)));
            }

            // On a page greater than the first page, but not the last
            if (pageCount > 1 & page > 1 & page < pageCount)
            {
                nextTag = String.Format(CanonicalNext, String.Format(AppConstants.PagingUrlFormat, url, (page + 1)));
                previousTag = String.Format(CanonicalPrev, String.Format(AppConstants.PagingUrlFormat, url, (page - 1)));
            }

            // On the last page
            if (pageCount > 1 & pageCount == page)
            {
                previousTag = String.Format(CanonicalPrev, String.Format(AppConstants.PagingUrlFormat, url, (page - 1)));
            }

            // return the canoncal tags
            return String.Concat(canonicalTag, Environment.NewLine,
                                    nextTag, Environment.NewLine,
                                    previousTag);
        }

        //public static string CreatePageTitle(Entity entity, string fallBack)
        //{
        //    if (entity != null)
        //    {
        //        if (entity is Category)
        //        {
        //            var cat = entity as Category;
        //            return cat.Name;
        //        }
        //        if (entity is Topic)
        //        {
        //            var topic = entity as Topic;
        //            return topic.Name;
        //        }
        //    }
        //    return fallBack;
        //}

        public static string CreateMetaDesc(Entity entity)
        {
            return "";
        }

        #endregion

        #region Urls

        public static bool Ping(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";

                using (var response = request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string CategoryRssUrls(string slug)
        {
            return $"/{SiteConstants.Instance.CategoryUrlIdentifier}/rss/{slug}";
        }

        public static string TopicUrls(string catSlug,string slug)
        {
            return $"/{SiteConstants.Instance.CategoryUrlIdentifier}/{catSlug}/{slug}";
        }

        public static string TopicUrls(Guid? cat, string slug)
        {
            string catSlug = "";

            if(cat != null)
            {
                var categoryService = ServiceFactory.Get<ICategoryService>();
                var cater = categoryService.Get((Guid)cat);

                if (cater != null) catSlug = cater.Slug;
            }

            return $"/{SiteConstants.Instance.CategoryUrlIdentifier}/{catSlug}/{slug}";
        }

        public static string ProductUrls(string catSlug, string slug)
        {
            return $"/{SiteConstants.Instance.ProductUrlIdentifier}/{catSlug}/{slug}";
        }

        public static string ProductUrls(Guid? cat, string slug)
        {
            string catSlug = "";

            if (cat != null)
            {
                var categoryService = ServiceFactory.Get<ICategoryService>();
                var cater = categoryService.Get((Guid)cat);

                if (cater != null) catSlug = cater.Slug;
            }

            return $"/{SiteConstants.Instance.ProductUrlIdentifier}/{catSlug}/{slug}";
        }


        #endregion

        #region String

        public static string ConvertPostContent(string post)
        {
            if (!string.IsNullOrEmpty(post))
            {
                // Convert any BBCode
                //NOTE: Decided to remove BB code
                //post = StringUtils.ConvertBbCodeToHtml(post, false);

                // If using the PageDown/MarkDown Editor uncomment this line
                post = StringUtils.ConvertMarkDown(post);

                // Allow video embeds
                post = StringUtils.EmbedVideosInPosts(post);

                // Add Google prettify code snippets
                post = post.Replace("<pre>", "<pre class='prettyprint'>");
            }

            return post;
        }

        public static string ReturnBadgeUrl(string badgeFile)
        {
            return string.Concat("~/content/badges/", badgeFile);
        }

        public static string ShotString(string text,int limit = 50)
        {
            return StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(text), limit);
        }
        #endregion

        #region Installer

        /// <summary>
        /// Get the previous version number if there is one from the web.config
        /// </summary>
        /// <returns></returns>
        public static string PreviousVersionNo()
        {
            return SiteConstants.Instance.MvcForumVersion;
        }

        /// <summary>
        /// Gets the main version number (Used by installer)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentVersionNo()
        {
            //Installer for new versions and first startup
            // Get the current version
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            // Store the value for use in the app
            return $"{version.Major}.{version.Minor}";
        }

        /// <summary>
        /// Get the full version number shown in the admin
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentVersionNoFull()
        {
            //Installer for new versions and first startup
            // Get the current version
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            // Store the value for use in the app
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        /// <summary>
        /// This checks whether the installer should be called, it stops people trying to call the installer
        /// when the application is already installed
        /// </summary>
        /// <returns></returns>
        public static bool ShowInstall()
        {
            //Installer for new versions and first startup
            // Store the value for use in the app
            var currentVersionNo = GetCurrentVersionNo();

            // Now check the version in the web.config
            var previousVersionNo = PreviousVersionNo();

            // If the versions are different kick the installer into play
            return (currentVersionNo != previousVersionNo);
        }

        #endregion

        #region Files

        public static bool FileIsImage(string file)
        {
            var imageFileTypes = new List<string>
            {
                ".jpg", ".jpeg",".gif",".bmp",".png"
            };
            return imageFileTypes.Any(file.Contains);
        }

        public static Image GetImageFromExternalUrl(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            using (var httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var stream = httpWebReponse.GetResponseStream())
                {
                    if (stream != null) return Image.FromStream(stream);
                }
            }
            return null;
        }

        public static string MemberImage(string avatar, string email, Guid userId, int size)
        {
            if (!string.IsNullOrEmpty(avatar))
            {
                // Has an avatar image
                var storageProvider = StorageProvider.Current;
                return storageProvider.BuildFileUrl(userId, "/", avatar, string.Format("?width={0}&crop=0,0,{0},{0}", size));
            }

            return StringUtils.GetGravatarImage(email, size);
        }

        public static string CategoryImage(string image, Guid categoryId, int size)
        {
            var sizeFormat = string.Format("?width={0}&crop=0,0,{0},{0}", size);
            if (!string.IsNullOrEmpty(image))
            {
                var storageProvider = StorageProvider.Current;
                return storageProvider.BuildFileUrl(categoryId, "/", image, sizeFormat);
            }
            //TODO - Return default image for category
            return null;
        }

        public static string CategoryImage(string image, Guid categoryId, int sizex,int sizey)
        {
            var sizeFormat = string.Format("?width={0}&crop=0,0,{0},{1}", sizex, sizey);
            if (!string.IsNullOrEmpty(image))
            {
                var storageProvider = StorageProvider.Current;
                return storageProvider.BuildFileUrl(categoryId, "/", image, sizeFormat);
            }
            //TODO - Return default image for category
            return null;
        }

        public static string CategoryImage(string image, Guid categoryId)
        {
            if (!string.IsNullOrEmpty(image))
            {
                var storageProvider = StorageProvider.Current;
                return storageProvider.BuildFileUrl(categoryId, "/", image);
            }
            //TODO - Return default image for category
            return null;
        }

        public static string CarouselImage(string image, Guid carouselId, int size)
        {
            var sizeFormat = string.Format("?width={0}&crop=0,0,{0},{0}", size);
            if (!string.IsNullOrEmpty(image))
            {
                var joinString = string.Concat(carouselId, "/", image, sizeFormat);
                return VirtualPathUtility.ToAbsolute(string.Concat(SiteConstants.Instance.UploadCarouselFolderPath, joinString));

            }
            //TODO - Return default image for category
            return null;
        }

        public static string CarouselImage(string image, Guid carouselId, int sizex, int sizey)
        {
            var sizeFormat = string.Format("?width={0}&height={1}&crop=0,0,{0},{1}", sizex, sizey);
            if (!string.IsNullOrEmpty(image))
            {
                var joinString = string.Concat(carouselId, "/", image, sizeFormat);
                return VirtualPathUtility.ToAbsolute(string.Concat(SiteConstants.Instance.UploadCarouselFolderPath, joinString));

            }
            //TODO - Return default image for category
            return null;
        }

        public static UploadFileResult UploadFile(HttpPostedFileBase file, string uploadFolderPath, ILocalizationService localizationService, bool onlyImages = false)
        {
            var upResult = new UploadFileResult { UploadSuccessful = true };
            const string imageExtensions = "jpg,jpeg,png,gif";
            var fileName = Path.GetFileName(file.FileName);
            var storageProvider = StorageProvider.Current;

            if (fileName != null)
            {
                // Lower case
                fileName = fileName.ToLower();

                // Get the file extension
                var fileExtension = Path.GetExtension(fileName);

                //Before we do anything, check file size
                if (file.ContentLength > Convert.ToInt32(SiteConstants.Instance.FileUploadMaximumFileSizeInBytes))
                {
                    //File is too big
                    upResult.UploadSuccessful = false;
                    upResult.ErrorMessage = localizationService.GetResourceString("Post.UploadFileTooBig");
                    return upResult;
                }

                // now check allowed extensions
                var allowedFileExtensions = SiteConstants.Instance.FileUploadAllowedExtensions;

                if (onlyImages)
                {
                    allowedFileExtensions = imageExtensions;
                }

                if (!string.IsNullOrEmpty(allowedFileExtensions))
                {
                    // Turn into a list and strip unwanted commas as we don't trust users!
                    var allowedFileExtensionsList = allowedFileExtensions.ToLower().Trim()
                                                     .TrimStart(',').TrimEnd(',').Split(',').ToList();

                    // If can't work out extension then just error
                    if (string.IsNullOrEmpty(fileExtension))
                    {
                        upResult.UploadSuccessful = false;
                        upResult.ErrorMessage = localizationService.GetResourceString("Errors.GenericMessage");
                        return upResult;
                    }

                    // Remove the dot then check against the extensions in the web.config settings
                    fileExtension = fileExtension.TrimStart('.');
                    if (!allowedFileExtensionsList.Contains(fileExtension))
                    {
                        upResult.UploadSuccessful = false;
                        upResult.ErrorMessage = localizationService.GetResourceString("Post.UploadBannedFileExtension");
                        return upResult;
                    }
                }

                // Store these here as we may change the values within the image manipulation
                var newFileName = string.Empty;
                var path = string.Empty;

                if (imageExtensions.Split(',').ToList().Contains(fileExtension))
                {
                    // Rotate image if wrong want around
                    using (var sourceimage = Image.FromStream(file.InputStream))
                    {
                        if (sourceimage.PropertyIdList.Contains(0x0112))
                        {
                            int rotationValue = sourceimage.GetPropertyItem(0x0112).Value[0];
                            switch (rotationValue)
                            {
                                case 1: // landscape, do nothing
                                    break;

                                case 8: // rotated 90 right
                                    // de-rotate:
                                    sourceimage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;

                                case 3: // bottoms up
                                    sourceimage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;

                                case 6: // rotated 90 left
                                    sourceimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                            }
                        }

                        using (var stream = new MemoryStream())
                        {
                            // Save the image as a Jpeg only
                            sourceimage.Save(stream, ImageFormat.Jpeg);
                            stream.Position = 0;

                            // Change the extension to jpg as that's what we are saving it as
                            fileName = fileName.Replace(fileExtension, "");
                            fileName = string.Concat(fileName, "jpg");
                            file = new MemoryFile(stream, "image/jpeg", fileName);

                            // Sort the file name
                            newFileName = CreateNewFileName(fileName);

                            // Get the storage provider and save file
                            upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, file);
                        }
                    }
                }
                else
                {
                    // Sort the file name
                    newFileName = CreateNewFileName(fileName);
                    upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, file);
                }

                upResult.UploadedFileName = newFileName;
            }

            return upResult;
        }

        private static string CreateNewFileName(string fileName)
        {
            return $"{GuidComb.GenerateComb()}_{fileName.Trim(' ').Replace("_", "-").Replace(" ", "-").ToLower()}";
        }

        #endregion

        #region Catergory
        public static Category Categorie(Guid id)
        {
            var categoryService = ServiceFactory.Get<ICategoryService>();
            return categoryService.Get(id);
        }

        public static List<Category> Categories()
        {
            var categoryService = ServiceFactory.Get<ICategoryService>();

            return categoryService.GetList();
        }

        public static List<Category> SubCategories(Category cat)
        {
            var categoryService = Categories();

            return categoryService.Where(x => x.Category_Id != null && x.Category_Id == cat.Id).ToList();
        }

        public static List<Category> Categories(bool isProduct)
        {
            var categoryService = ServiceFactory.Get<ICategoryService>();

            return categoryService.GetList(isProduct);
        }

        public static List<Category> SubCategories(Category cat,bool isProduct)
        {
            var categoryService = Categories(isProduct);

            return categoryService.Where(x => x.Category_Id != null && x.Category_Id == cat.Id).ToList();
        }
        #endregion

        #region Topic
        public static Topic GetTopic(Guid Id)
        {
            var topicService = ServiceFactory.Get<ITopicService>();

            return topicService.Get(Id);
        }

        public static int TopicCount()
        {
            var topicService = ServiceFactory.Get<ITopicService>();

            return topicService.GetCount();
        }

        public static List<Topic> TopTopics(int limit,int page = 1)
        {
            var topicService = ServiceFactory.Get<ITopicService>();

            return topicService.GetList(limit, page);
        }

        public static List<Topic> TopTopics(Category cat,int limit, int page = 1)
        {
            var topicService = ServiceFactory.Get<ITopicService>();

            return topicService.GetList(cat.Id,limit, page);
        }

        public static List<Topic> HostTopics(int limit, int page = 1)
        {
            var topicService = ServiceFactory.Get<ITopicService>();

            return topicService.GetList(limit, page);
        }

        public static List<Topic> HostViewTopics(int limit, int page = 1)
        {
            var topicService = ServiceFactory.Get<ITopicService>();

            return topicService.GetList(limit, page);
        }
        #endregion

        #region Product
        public static string GetNameProduct(Guid id)
        {
            try
            {
                return GetProduct(id).Name;
            }
            catch
            {
                return "";
            }
        }

        public static Product GetProduct(Guid id)
        {
            var productService = ServiceFactory.Get<IProductSevice>();
            return productService.Get(id);
        }

        public static List<Product> Products(int limit)
        {
            var productService = ServiceFactory.Get<IProductSevice>();

            return productService.GetList(limit);
        }

        public static List<Product> Products(Category cat,int limit)
        {
            var productService = ServiceFactory.Get<IProductSevice>();

            return productService.GetList(cat, limit);
        }

        public static List<Product> HostProducts(Category cat, int limit)
        {
            var productService = ServiceFactory.Get<IProductSevice>();

            return productService.GetList(cat, limit);
        }

        public static List<Product> SellingProducts(Category cat, int limit)
        {
            var productService = ServiceFactory.Get<IProductSevice>();

            return productService.GetList(cat, limit);
        }

        //public static List<Product> Products(List<Category> cats, int limit)
        //{
        //    var productService = ServiceFactory.Get<IProductSevice>();

        //    return productService.GetList(cats, limit);
        //}

        public static Hashtable ProductValues(Product product)
        {
            var productService = ServiceFactory.Get<IProductSevice>();
            //var value = new List<ProductValueViewModel>();

            var value = new Hashtable();

            var property = productService.GetListProductClassAttributeForProductClassId((Guid)product.ProductClassId);
            foreach(var it in property)
            {
                var a = new ProductValueViewModel
                {
                    IsShow = it.IsShow
                };

                var att = productService.GetAttribute(it.ProductAttributeId);
                var v = productService.GetAttributeValue(product.Id, it.ProductAttributeId);

                if(v != null)
                    a.Value = v.Value;

                if(att != null)
                    a.Name = att.LangName;

                value.Add(a.Name,a);
            }

            return value;
        }

        public static List<ProductClassAttribute> ProductAttributes(Product product)
        {
            var productService = ServiceFactory.Get<IProductSevice>();

            

            return productService.GetListProductClassAttributeForProductClassId((Guid)product.Category_Id);
        }
        #endregion
        #region ProductClass
        public static ProductClass GetGroupProduct(Guid id)
        {
            var productService = ServiceFactory.Get<IProductSevice>();
            return productService.GetProductClass(id);
        }

        public static string GetNameGroupProduct(Guid id)
        {
            try
            {
                return GetGroupProduct(id).Name;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region Members
        public static string GetMembersAccount(Guid id)
        {
            try
            {
                return GetMembers(id).UserName;
            }
            catch
            {
                return "";
            }
        }

        public static MembershipUser GetMembers(Guid id)
        {
            var membershipService = ServiceFactory.Get<IMembershipService>();
            return membershipService.Get(id);
        }
        #endregion

        #region ShoppingCart
        public static string GetJsonShoppingCart()
        {
            Hashtable list = null;
            try
            {
                list = (Hashtable)HttpContext.Current.Session["SopiingCart"];
            }
            catch { }

            if (list == null)
            {
                list = new Hashtable();
                HttpContext.Current.Session["SopiingCart"] = list;
            }

            var viewModel = new CartListViewModel();
            viewModel.Count = list.Count;

            viewModel.Products = new List<CartItemViewModel>();
            foreach (DictionaryEntry it in list)
            {
                viewModel.Products.Add((CartItemViewModel)it.Value);
            }

            return JsonConvert.SerializeObject(viewModel);
        }

        public static int GetCountShoppingCart()
        {
            int i = 0;
            try
            {
                i = ((Hashtable)HttpContext.Current.Session["SopiingCart"]).Count;
            }
            catch {}

            return i;
        }
        #endregion

        #region Employees
        public static string GetNameEmployeesRole(Guid Id)
        {
            var emp = ServiceFactory.Get<IEmployeesRoleService>();

            return emp.Get(Id).Name;
        }
        #endregion

        #region Showroom
        public static List<AjaxShowroomItemViewModel> GetShowroom()
        {
            var key = "AppHelpers.GetShowroom";
            var ListShowrooms = (List<AjaxShowroomItemViewModel>)HttpContext.Current.Items[key];
            if (ListShowrooms != null) return ListShowrooms;

            ListShowrooms = new List<AjaxShowroomItemViewModel>();

            var SettingsService = ServiceFactory.Get<ISettingsService>();
            var ShowroomCount = SettingsService.GetSetting("ShowroomCount");
            int count = 0;
            try
            {
                count = int.Parse(ShowroomCount);
            }
            catch { }

            for (int i = 0; i < count; i++)
            {
                ListShowrooms.Add(new AjaxShowroomItemViewModel
                {
                    Addren = SettingsService.GetSetting("Showroom[" + i + "].Address"),
                    iFrameMap = SettingsService.GetSetting("Showroom[" + i + "].iFrameMap"),
                });
            }

            HttpContext.Current.Items[key] = ListShowrooms;
            return ListShowrooms;
        }
        #endregion

        #region Menu
        public static List<Menu> GetAllMenus()
        {
            var MenuService = ServiceFactory.Get<IMenuService>();
            return MenuService.GetAll();
        }

        public static string GetLinkByMenu(Menu menu)
        {
            if (menu.Link.IsNullEmpty()) return "/";  

            switch (menu.iType)
            {
                case 0:
                    return menu.Link;
                case 1:
                    //var context = HttpContext.Current.
                    switch(menu.Link)
                    {
                        case "0":
                            return "/";
                        case "1":
                            return "/tin-tuc";
                        case "2":
                            return "/san-pham";
                        case "3":
                            return "/lien-he";
                    }
                    break;
                case 2:
                    var cat = Categorie(new Guid(menu.Link));
                    if(cat != null)
                    {
                        if (cat.IsProduct)
                        {
                            return string.Concat("/", SiteConstants.Instance.ProductUrlIdentifier, "/", cat.Slug);
                        }
                        return string.Concat("/", SiteConstants.Instance.CategoryUrlIdentifier, "/", cat.Slug); 
                    }
                    break;
                case 3:
                    var news = GetTopic(new Guid(menu.Link));
                    if (news != null && news.Category_Id != null)
                    {
                        var cat1 = Categorie((Guid)news.Category_Id);

                        return string.Concat("/", SiteConstants.Instance.CategoryUrlIdentifier, "/", cat1.Slug,"/",news.Slug);
                    }
                    break;
                case 4:
                    var prod = GetProduct(new Guid(menu.Link));
                    if (prod != null && prod.Category_Id != null)
                    {
                        var cat1 = Categorie((Guid)prod.Category_Id);

                        return string.Concat("/", SiteConstants.Instance.ProductUrlIdentifier, "/", cat1.Slug, "/", prod.Slug);
                    }
                    break;
            }


            return "/";
        }
        #endregion
    }
}