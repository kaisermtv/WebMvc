using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class MenuService : IMenuService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILocalizationService _localizationService;

        public MenuService(IWebMvcContext context, ICacheService cacheService, ILocalizationService localizationService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
            _localizationService = localizationService;
        }



        #region DataRowToMenu
        private Menu DataRowToMenu(DataRow data)
        {
            if (data == null) return null;

            var menu = new Menu();

            menu.Id = new Guid(data["Id"].ToString());
            menu.Name = data["Name"].ToString();
            menu.Description = data["Description"].ToString();
            menu.Colour = data["Colour"].ToString();
            menu.Image = data["Image"].ToString();
            if(data["iType"] != DBNull.Value) menu.iType = (int)data["iType"];
            menu.Link = data["Link"].ToString();
            menu.Image = data["Image"].ToString();

            if(data["Menu_Id"] != DBNull.Value) menu.Menu_Id = new Guid(data["Menu_Id"].ToString());

            return menu;
        }
        #endregion


        public void Add(Menu menu)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "INSERT INTO [dbo].[Menu]([Id],[Menu_Id],[Name],[Description],[iType],[Link],[Image],[Colour],[SortOrder])"
                + " VALUES(@Id,@Menu_Id,@Name,@Description,@iType,@Link,@Image,@Colour,@SortOrder)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = menu.Id;
            Cmd.AddParameters("Name", menu.Name);
            Cmd.AddParameters("Description", menu.Description);
            Cmd.AddParameters("Colour", menu.Colour);
            Cmd.AddParameters("iType", menu.iType);
            Cmd.AddParameters("Link", menu.Link);
            Cmd.AddParameters("Image", menu.Image);
            Cmd.AddParameters("SortOrder", menu.SortOrder);
            Cmd.AddParameters("Menu_Id", menu.Menu_Id);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Menu.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Menu false");
        }


        public void Update(Menu menu)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[Menu] SET [Menu_Id] = @Menu_Id,[Name] = @Name,[Description] = @Description,[iType] = @iType,"
                                + "[Link] = @Link,[Image] = @Image,[Colour] = @Colour,[SortOrder] = @SortOrder WHERE [Id] = @Id";


            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = menu.Id;
            Cmd.AddParameters("Name", menu.Name);
            Cmd.AddParameters("Description", menu.Description);
            Cmd.AddParameters("Colour", menu.Colour);
            Cmd.AddParameters("iType", menu.iType);
            Cmd.AddParameters("Link", menu.Link);
            Cmd.AddParameters("Image", menu.Image);
            Cmd.AddParameters("SortOrder", menu.SortOrder);
            Cmd.AddParameters("Menu_Id", menu.Menu_Id);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Menu.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update Menu false");
        }
        
        public void Del(Menu menu)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Menu] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = menu.Id;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Menu.StartsWith);
            Cmd.Close();
        }

        public Menu Get(string id)
        {
            return Get(new Guid(id));
        }
        public Menu Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Menu.StartsWith, "Get-", id);

            var cat = _cacheService.Get<Menu>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (Menu it in allcat)
                {
                    if (it.Id == id)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }


        public List<Menu> GetSubMenus(Menu cat)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetSubMenus", "-", cat);
            var list = _cacheService.Get<List<Menu>>(cacheKey);
            if (list == null)
            {
                var cats = GetAll();
                list = new List<Menu>();

                int i = 0, x = 0;
                while (true)
                {
                    if (cats[i].Menu_Id == cat.Id)
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

                _cacheService.Set(cacheKey, list, CacheTimes.OneMinute);
            }

            return list;
        }

        public List<Menu> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Menu.StartsWith, "GetAll");

            var allCat = _cacheService.Get<List<Menu>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[Menu] ORDER BY SortOrder ASC";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<Menu>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToMenu(it));
                }


                foreach (var it in allCat)
                {
                    if (it.Menu_Id == null)
                    {
                        SetLiveMenu(it, allCat);
                    }
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);
            }
            return allCat;
        }


        private void SetLiveMenu(Menu cat, List<Menu> allcat, int leve = 2)
        {
            foreach (var it in allcat)
            {
                if (cat.Id == it.Menu_Id)
                {
                    it.Level = leve;
                    SetLiveMenu(it, allcat, leve + 1);
                }
            }
        }


        public List<SelectListItem> GetBaseSelectListMenus(List<Menu> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetBaseSelectListMenus-", allowedCategories.GetHashCode());
            var list = _cacheService.Get<List<SelectListItem>>(cacheKey);
            if(list == null)
            {
                list = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedCategories)
                {
                    var catName = string.Concat(LevelDashes(cat.Level), cat.Level > 1 ? " " : "", cat.Name);
                    list.Add(new SelectListItem { Text = catName, Value = cat.Id.ToString() });
                }
                

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        public List<Menu> GetAllSubMenus(Menu cat, List<Menu> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetAllSubMenus", "-", cat, "-", allowedCategories.GetHashCode());
            var list = _cacheService.Get<List<Menu>>(cacheKey);
            if (list == null)
            {
                list = new List<Menu>();

                int i = 0, x = 0;
                while (true)
                {
                    if (allowedCategories[i].Menu_Id == cat.Id)
                    {
                        list.Add(allowedCategories[i]);
                    }

                    i++;
                    if (i >= allowedCategories.Count)
                    {
                        if (x >= list.Count) break;
                        cat = list[x];
                        x++;
                        i = 0;
                    }
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }

            return list;
        }

        #region GetMenusParenMenu

        public List<Menu> GetMenusParenMenu(Menu cat)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetMenusParenMenu", "-", cat);
            var list = _cacheService.Get<List<Menu>>(cacheKey);
            if (list == null)
            {
                var cats = GetAll();
                list = new List<Menu>(cats);
                list.Remove(cat);

                var sublist = GetAllSubMenus(cat, cats);

                foreach (Menu it in sublist)
                {
                    list.Remove(it);
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }
        #endregion




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
