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

namespace WebMvc.Services
{
    public partial class EmployeesService : IEmployeesService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public EmployeesService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEmployees
        private Employees DataRowToEmployees(DataRow data)
        {
            if (data == null) return null;

            var emp = new Employees();

            emp.Id = new Guid(data["Id"].ToString());
            emp.RoleId = new Guid(data["RoleId"].ToString());
            emp.Name = data["Name"].ToString();
            emp.Phone = data["Phone"].ToString();
            emp.Email = data["Email"].ToString();
            emp.Skype = data["Skype"].ToString();

            return emp;
        }
        #endregion


        public void Add(Employees emp)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [Employees]([Id],[RoleId],[Name],[Phone],[Email],[Skype])"
                + " VALUES(@Id,@RoleId,@Name,@Phone,@Email,@Skype)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = emp.Id;
            Cmd.Parameters.Add("RoleId", SqlDbType.UniqueIdentifier).Value = emp.RoleId;
            Cmd.AddParameters("Name", emp.Name);
            Cmd.AddParameters("Phone", emp.Phone);
            Cmd.AddParameters("Email", emp.Email);
            Cmd.AddParameters("Skype", emp.Skype);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Employees.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Employees false");
        }

        public void Update(Employees emp)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[Employees] SET [RoleId] = @RoleId,[Name] = @Name,[Phone] = @Phone,[Email] = @Email,[Skype] = @Skype WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = emp.Id;
            Cmd.Parameters.Add("RoleId", SqlDbType.UniqueIdentifier).Value = emp.RoleId;
            Cmd.AddParameters("Name", emp.Name);
            Cmd.AddParameters("Phone", emp.Phone);
            Cmd.AddParameters("Email", emp.Email);
            Cmd.AddParameters("Skype", emp.Skype);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Employees.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update Employees false");
        }


        public void Del(Employees emp)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Employees] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = emp.Id;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Employees.StartsWith);
            Cmd.Close();

        }

        public Employees Get(string id)
        {
            return Get(new Guid(id));
        }
        public Employees Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Employees.StartsWith, "Get-", id);

            var cat = _cacheService.Get<Employees>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (Employees it in allcat)
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


        public List<Employees> GetList(EmployeesRole employeesRole)
        {
            string cachekey = string.Concat(CacheKeys.Employees.StartsWith, "GetList-", employeesRole.Id);

            var allCat = _cacheService.Get<List<Employees>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [Employees] WHERE RoleId = @RoleId";
                Cmd.Parameters.Add("RoleId", SqlDbType.UniqueIdentifier).Value = employeesRole.Id;

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<Employees>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToEmployees(it));
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneHour);

                //_cacheService.ClearStartsWith(string.Concat(CacheKeys.Category.StartsWith, "GetList-"));

            }
            return allCat;
        }

        public List<Employees> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Employees.StartsWith, "GetAll");

            var allCat = _cacheService.Get<List<Employees>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [Employees]";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<Employees>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToEmployees(it));
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);

                //_cacheService.ClearStartsWith(string.Concat(CacheKeys.Category.StartsWith, "GetList-"));

            }
            return allCat;
        }
    }
}
