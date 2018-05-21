using System;
using System.Collections.Generic;
using System.Data;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;


namespace WebMvc.Services
{
    public partial class PermissionService : IPermissionService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public PermissionService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }


        #region DataRowToEntity
        private Permission DataRowToPermission(DataRow data)
        {
            if (data == null) return null;

            Permission role = new Permission();

            role.Id = new Guid(data["Id"].ToString());
            role.Name = data["Name"].ToString();
            role.IsGlobal = (bool)data["IsGlobal"];


            return role;
        }
        #endregion

        #region MembershipRole Action
        public void Add(Permission role)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Permission] WHERE [Id] = @Id OR [Name] = @Name)";
            Cmd.CommandText += " BEGIN INSERT INTO [Permission]([Id],[Name],[IsGlobal])";
            Cmd.CommandText += " VALUES(@Id,@Name,@IsGlobal) END ";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = role.Id;
            Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = role.Name;
            Cmd.Parameters.Add("IsGlobal", SqlDbType.NVarChar).Value = role.IsGlobal;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Add Permission false");
        }

        public List<Permission> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Permission.StartsWith, "GetAll");
            var cachedSettings = _cacheService.Get<List<Permission>>(cachekey);
            if (cachedSettings == null)
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [Permission]";

                DataTable data = Cmd.findAll();
                Cmd.Close();
                if (data == null) return null;

                cachedSettings = new List<Permission>();
                foreach (DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToPermission(it));
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }

        public Permission Get(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.Permission.StartsWith, "Get-",Id);
            var cachedSettings = _cacheService.Get<Permission>(cachekey);
            if (cachedSettings == null)
            {

                var data = GetAll();

                foreach (var it in data)
                {
                    if (it.Id == Id)
                    {
                        cachedSettings = it;
                        break;
                    }
                }
                if (cachedSettings == null) return null;

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }

        public string GetName(Guid Id)
        {
            var data = Get(Id);

            if (data == null) return null;
            return data.Name;
        }

        public void Update(Permission role)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);

            Cmd.CommandText = "UPDATE [Permission] SET [Name] = @Name, [IsGlobal] = @IsGlobal WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = role.Id;
            Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = role.Name;
            Cmd.Parameters.Add("IsGlobal", SqlDbType.NVarChar).Value = role.IsGlobal;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update Permission false");
        }

        public void Delete(Guid Id)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

            Cmd.CommandText = "DELETE FROM [Permission] WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update Permission false");
        }
        #endregion
    }
}
