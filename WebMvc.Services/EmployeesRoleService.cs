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

namespace WebMvc.Services
{
    public partial class EmployeesRoleService : IEmployeesRoleService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public EmployeesRoleService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEmployeesRole
        private EmployeesRole DataRowToEmployeesRole(DataRow data)
        {
            if (data == null) return null;

            var cat = new EmployeesRole();

            cat.Id = new Guid(data["Id"].ToString());
            cat.Name = data["Name"].ToString();
            cat.Description = data["Description"].ToString();

            return cat;
        }
        #endregion


        public void Add(EmployeesRole role)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "INSERT INTO [EmployeesRole]([Id],[Name],[Description])"
                + " VALUES(@Id,@Name,@Description)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = role.Id;
            Cmd.AddParameters("Name", role.Name);
            Cmd.AddParameters("Description", role.Description);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.EmployeesRole.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add EmployeesRole false");
        }

        public void Update(EmployeesRole role)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[EmployeesRole] SET [Name] = @Name, [Description] = @Description WHERE [Id] = @Id";
            
            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = role.Id;
            Cmd.AddParameters("Name", role.Name);
            Cmd.AddParameters("Description", role.Description);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.EmployeesRole.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update EmployeesRole false");
        }

        public EmployeesRole Get(string id)
        {
            return Get(new Guid(id));
        }
        public EmployeesRole Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.EmployeesRole.StartsWith, "Get-", id);

            var cat = _cacheService.Get<EmployeesRole>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (EmployeesRole it in allcat)
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

        public List<EmployeesRole> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.EmployeesRole.StartsWith, "GetAll");

            var allCat = _cacheService.Get<List<EmployeesRole>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [EmployeesRole] ORDER BY SortOrder ASC";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<EmployeesRole>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToEmployeesRole(it));
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);

                //_cacheService.ClearStartsWith(string.Concat(CacheKeys.Category.StartsWith, "GetList-"));

            }
            return allCat;
        }


        public List<SelectListItem> GetBaseSelectListEmployeesRole(List<EmployeesRole> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.EmployeesRole.StartsWith, "GetBaseSelectListEmployeesRole", "-", allowedCategories.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedCategories)
                {
                    cats.Add(new SelectListItem { Text = cat.Name, Value = cat.Id.ToString() });
                }
                return cats;
            });
        }

    }
}
