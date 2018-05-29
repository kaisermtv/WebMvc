using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;

namespace WebMvc.Services
{
    public partial class CategoryService : ICategoryService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public CategoryService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }


        #region DataRowToCategory
        private Category DataRowToCategory(DataRow data)
        {
            if (data == null) return null;

            Category cat = new Category();

            cat.Id = new Guid(data["Id"].ToString());
            cat.Name = data["Name"].ToString();
            cat.Description = data["Description"].ToString();
            cat.IsLocked = (bool)data["IsLocked"];
            cat.ModerateTopics = (bool)data["ModerateTopics"];
            cat.ModeratePosts = (bool)data["ModeratePosts"];
            cat.SortOrder = (int)data["SortOrder"];
            cat.DateCreated = (DateTime)data["DateCreated"];
            cat.Slug = data["Slug"].ToString();
            cat.PageTitle = data["PageTitle"].ToString();
            cat.Path = data["Path"].ToString();
            cat.MetaDescription = data["MetaDescription"].ToString();
            cat.Colour = data["Colour"].ToString();
            cat.Image = data["Image"].ToString();
            
            if (data["IsProduct"] != DBNull.Value)
            {
                cat.IsProduct = (bool)data["IsProduct"];
            }
            else
            {
                cat.IsProduct = false;
            }

            string catid = data["Category_Id"].ToString();
            if (!catid.IsNullEmpty())
                cat.Category_Id = new Guid(data["Category_Id"].ToString());


            return cat;
        }
        #endregion

        #region Slug
        private bool CheckSlug(string slug)
        {
            var lst = GetAll();
            foreach (var it in lst)
            {
                if (slug == it.Slug) return false;
            }

            return true;
        }

        private bool TestSlug(string slug, string newslug)
        {
            if (slug == null) slug = "";
            return Regex.IsMatch(slug, string.Concat("^", newslug, "(-[\\d]+)?$"));
        }

        private void CreateSlug(Category cat)
        {
            var slug = ServiceHelpers.CreateUrl(cat.Name);
            if(!TestSlug(cat.Slug, slug))
            {
                var tmpSlug = slug;

                int i = 0;
                while (!CheckSlug(tmpSlug))
                {
                    i++;
                    tmpSlug = string.Concat(slug,"-",i);
                }

                cat.Slug = tmpSlug;
            }
        }
        #endregion

        public void Add(Category cat)
        {
            //string cachekey = string.Concat(CacheKeys.Category.StartsWith, "getSetting-", key);

            var Cmd = _context.CreateCommand();

            cat.DateCreated = DateTime.UtcNow;
            CreateSlug(cat);
            

            Cmd.CommandText = "INSERT INTO [Category]([Id],[Name],[Description],[IsLocked],[ModerateTopics],[ModeratePosts],[SortOrder]"
                + ",[DateCreated],[Slug],[PageTitle],[Path],[MetaDescription],[Colour],[Image],[Category_Id],[IsProduct])"
                + " VALUES(@Id,@Name,@Description,@IsLocked,@ModerateTopics,@ModeratePosts,@SortOrder"
                + ",@DateCreated,@Slug,@PageTitle,@Path,@MetaDescription,@Colour,@Image,@Category_Id,@IsProduct)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Description", cat.Description);
            Cmd.AddParameters("IsLocked", cat.IsLocked);
            Cmd.AddParameters("ModerateTopics", cat.ModerateTopics);
            Cmd.AddParameters("ModeratePosts", cat.ModeratePosts);
            Cmd.AddParameters("SortOrder", cat.SortOrder);
            Cmd.AddParameters("DateCreated", cat.DateCreated);
            Cmd.AddParameters("Slug", cat.Slug);
            Cmd.AddParameters("PageTitle", cat.PageTitle);
            Cmd.AddParameters("Path", cat.Path);
            Cmd.AddParameters("MetaDescription", cat.MetaDescription);
            Cmd.AddParameters("Colour", cat.Colour);
            Cmd.AddParameters("Image", cat.Image);
            Cmd.AddParameters("Category_Id", cat.Category_Id);
            Cmd.AddParameters("IsProduct", cat.IsProduct);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Category.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Category false");
        }

        public void Update(Category cat)
        {
            var Cmd = _context.CreateCommand();
            CreateSlug(cat);

            Cmd.CommandText = "UPDATE [dbo].[Category] SET [Name] = @Name, [Description] = @Description, [IsLocked] = @IsLocked,"
                + "[ModerateTopics] = @ModerateTopics, [ModeratePosts] = @ModeratePosts,[SortOrder] = @SortOrder,"
                + "[DateCreated] = @DateCreated,[Slug] = @Slug,[PageTitle] = @PageTitle,[Path] = @Path,"
                + "[MetaDescription] = @MetaDescription,[Colour] = @Colour,[Image] = @Image,[Category_Id] = @Category_Id, [IsProduct] = @IsProduct"
                + " WHERE Id = @Id";


            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Description", cat.Description);
            Cmd.AddParameters("IsLocked", cat.IsLocked);
            Cmd.AddParameters("ModerateTopics", cat.ModerateTopics);
            Cmd.AddParameters("ModeratePosts", cat.ModeratePosts);
            Cmd.AddParameters("SortOrder", cat.SortOrder);
            Cmd.AddParameters("DateCreated", cat.DateCreated);
            Cmd.AddParameters("Slug", cat.Slug);
            Cmd.AddParameters("PageTitle", cat.PageTitle);
            Cmd.AddParameters("Path", cat.Path);
            Cmd.AddParameters("MetaDescription", cat.MetaDescription);
            Cmd.AddParameters("Colour", cat.Colour);
            Cmd.AddParameters("Image", cat.Image);
            Cmd.AddParameters("Category_Id", cat.Category_Id);
            Cmd.AddParameters("IsProduct", cat.IsProduct);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Category.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update Category false");
        }

        public Category Get(string id)
        {
            return Get(new Guid(id));
        }
        public Category Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Category.StartsWith, "Get-", id);

            var cat = _cacheService.Get<Category>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;
                
                foreach (Category it in allcat)
                {
                    if(it.Id == id)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }

        public Category GetBySlug (string slug)
        {
            string cachekey = string.Concat(CacheKeys.Category.StartsWith, "GetBySlug-", slug);

            var cat = _cacheService.Get<Category>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (Category it in allcat)
                {
                    if (it.Slug == slug)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }

        public List<Category> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Category.StartsWith, "GetAll");

            var allCat = _cacheService.Get<List<Category>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [Category] ORDER BY SortOrder ASC";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<Category>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToCategory(it));
                }

                foreach (Category it in allCat)
                {
                    if (it.Category_Id == null)
                    {
                        SetLiveCat(it, allCat);
                    }
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);

                //_cacheService.ClearStartsWith(string.Concat(CacheKeys.Category.StartsWith, "GetList-"));

            }
            return allCat;
        }

        private void SetLiveCat(Category cat, List<Category> allcat,int leve = 2)
        {
            foreach(Category it in allcat)
            {
                if(cat.Id == it.Category_Id)
                {
                    it.Level = leve;
                    SetLiveCat(it, allcat, leve+1);
                }
            }
        }

        public List<Category> GetList(Guid? paren = null)
        {
            string cachekey = string.Concat(CacheKeys.Category.StartsWith, "GetList-", paren);

            var list = _cacheService.Get<List<Category>>(cachekey);
            if (list == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                list = new List<Category>();
                
                foreach (Category it in allcat)
                {
                    if(it.Category_Id == paren)
                    {
                        list.Add(it);
                    }
                }
                
                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public List<Category> GetList( bool isProduct, Guid? paren)
        {
            string cachekey = string.Concat(CacheKeys.Category.StartsWith, "GetList-", paren,"-", isProduct);

            var list = _cacheService.Get<List<Category>>(cachekey);
            if (list == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                list = new List<Category>();

                foreach (Category it in allcat)
                {
                    if (it.Category_Id == paren && it.IsProduct == isProduct)
                    {
                        list.Add(it);
                    }
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public List<Category> GetList(bool isProduct)
        {
            string cachekey = string.Concat(CacheKeys.Category.StartsWith, "GetList-", isProduct);

            var list = _cacheService.Get<List<Category>>(cachekey);
            if (list == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                list = new List<Category>();

                foreach (Category it in allcat)
                {
                    if (it.IsProduct == isProduct)
                    {
                        list.Add(it);
                    }
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public List<SelectListItem> GetBaseSelectListCategories(List<Category> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "GetBaseSelectListCategories", "-", allowedCategories.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedCategories)
                {
                    var catName = string.Concat(LevelDashes(cat.Level), cat.Level > 1 ? " " : "", cat.Name);
                    cats.Add(new SelectListItem { Text = catName, Value = cat.Id.ToString() });
                }
                return cats;
            });
        }

        public List<Category> GetAllSubCategories(Category cat, List<Category> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "GetAllSubCategories", "-", cat,"-", allowedCategories.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = GetAll();
                var list = new List<Category>();
                
                int i = 0, x = 0;
                while (true)
                {
                    if (cats[i].Category_Id == cat.Id)
                    {
                        list.Add(cats[i]);
                    }
                    
                    i++;
                    if (i >= cats.Count)
                    {
                        if (x >= list.Count) break;
                        cat = list[x];
                        x++;
                        i = 0;
                    }
                }

                return list;
            });


        }

        #region GetCategoriesParenCatregori
      
        public List<Category> GetCategoriesParenCatregori(Category cat)
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "GetCategoriesParenCatregori", "-", cat);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = GetAll();
                var list = new List<Category>(cats);
                list.Remove(cat);

                var sublist = GetAllSubCategories(cat, cats);

                foreach (Category it in sublist)
                {
                    list.Remove(it);
                }

                return list;
            });
        }
        #endregion

        public List<Category> GetAllowedCategories(Guid Role)
        {
            return GetAll();
        }

        public List<Category> GetAllowedEditCategories(Guid Role)
        {
            return GetAll();
        }

        public List<Category> GetAllowedCategories(Guid Role, bool IsProduct)
        {
            List<Category> lst = new List<Category>();

            foreach (var it in GetAll())
            {
                if(it.IsProduct == IsProduct)
                {
                    lst.Add(it);
                }
            }

            return lst;
        }

        public List<Category> GetAllowedEditCategories(Guid Role, bool IsProduct)
        {
            List<Category> lst = new List<Category>();

            foreach (var it in GetAll())
            {
                if (it.IsProduct == IsProduct)
                {
                    lst.Add(it);
                }
            }

            return lst;
        }

        private static string LevelDashes(int level)
        {
            if (level > 1)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < level - 1; i++)
                {
                    sb.Append("-");
                }
                return sb.ToString();
            }
            return string.Empty;
        }
        
    }
}
