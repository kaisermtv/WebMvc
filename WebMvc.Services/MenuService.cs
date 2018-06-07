using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            menu.Image = data["Image"].ToString();

            string menuid = data["Menu_Id"].ToString();
            if (!menuid.IsNullEmpty())
                menu.Menu_Id = new Guid(menuid);
            
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

            Cmd.CommandText = "UPDATE [dbo].[Category] SET [Menu_Id] = @Menu_Id,[Name] = @Name,[Description] = @Description,[iType] = @iType,"
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
                
                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);
            }
            return allCat;
        }



    }
}
