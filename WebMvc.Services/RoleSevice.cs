namespace WebMvc.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using WebMvc.Domain.Constants;
    using WebMvc.Domain.DomainModel.Entities;
    using WebMvc.Domain.DomainModel.Enums;
    using WebMvc.Domain.Interfaces;
    using WebMvc.Domain.Interfaces.Services;
    using WebMvc.Services.Data.Context;

    public partial class RoleSevice : IRoleSevice
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public RoleSevice(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private MembershipRole DataRowToMembershipRole(DataRow data)
        {
            if (data == null) return null;

            MembershipRole role = new MembershipRole();

            role.Id = new Guid(data["Id"].ToString());
            role.RoleName = data["RoleName"].ToString();


            return role;
        }
        #endregion

        #region MembershipRole Action
        public void AddMembershipRole(MembershipRole role)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [MembershipRole] WHERE [Id] = @Id OR [RoleName] = @RoleName)";
            Cmd.CommandText += " BEGIN INSERT INTO [MembershipRole]([Id],[RoleName])";
            Cmd.CommandText += " VALUES(@Id,@RoleName) END ";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = role.Id;
            Cmd.Parameters.Add("RoleName", SqlDbType.NVarChar).Value = role.RoleName;
            //Cmd.AddParameters("Id", role.Id);
            //Cmd.AddParameters("RoleName", role.RoleName);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Add MembershipRole false");
        }

        public List<MembershipRole> GetAllMembershipRole()
        {
            string cachekey = string.Concat(CacheKeys.Role.MembershipRole, "GetAllMembershipRole");
            var cachedSettings = _cacheService.Get<List<MembershipRole>>(cachekey);
            if (cachedSettings == null)
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [MembershipRole]";

                DataTable data = Cmd.findAll();
                Cmd.Close();
                if (data == null) return null;

                cachedSettings = new List<MembershipRole>();
                foreach(DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToMembershipRole(it));
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }
            
            return cachedSettings;
        }

        public MembershipRole GetMembershipRole(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.Role.MembershipRole, "GetAllMembershipRole-",Id);
            var cachedSettings = _cacheService.Get<MembershipRole>(cachekey);
            if (cachedSettings == null)
            {

                var data = GetAllMembershipRole();
                
                foreach(var it in data)
                {
                    if(it.Id == Id)
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

        public string GetNameMembershipRole(Guid Id)
        {
            var data = GetMembershipRole(Id);

            if (data == null) return null;
            return data.RoleName;
        }

        public void UpdateMembershipRole(MembershipRole role)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

            Cmd.CommandText = "UPDATE [MembershipRole] SET [RoleName] = @RoleName WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = role.Id;
            Cmd.Parameters.Add("RoleName", SqlDbType.NVarChar).Value = role.RoleName;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update MembershipRole false");
        }

        public void DeleteMembershipRole(Guid Id)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

            Cmd.CommandText = "DELETE FROM [MembershipRole] WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update MembershipRole false");
        }
        #endregion
    }
}
