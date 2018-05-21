using System;
using System.Data;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;

namespace WebMvc.Services
{
    public partial class SettingsService : ISettingsService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public SettingsService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        public string GetSetting(string key)
        {
            string cachekey = string.Concat(CacheKeys.Settings.StartsWith, "getSetting-", key);

            var cachedSettings = _cacheService.Get<string>(cachekey);
            if (cachedSettings == null)
            {
                cachedSettings = this.GetSettingNoCache(key);
                
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }
            return cachedSettings;
        }

        public string GetSettingNoCache(string key)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "SELECT [VALUE] FROM tblSetting WHERE STKEY = @STKEY";
            Cmd.Parameters.Add("STKEY", SqlDbType.NVarChar).Value = key;

            var rt = Cmd.command.ExecuteScalar();

            string cachedSettings = null;
            if (rt != null) cachedSettings = Cmd.command.ExecuteScalar().ToString();
            
            Cmd.Close();

            return cachedSettings;
        }

        public void SetSetting(string key,string value)
        {
            string cachekey = string.Concat(CacheKeys.Settings.StartsWith, "getSetting-", key);

            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM tblSetting WHERE STKEY = @STKEY)";
            Cmd.CommandText += " BEGIN INSERT INTO tblSetting(STKEY,VALUE) VALUES(@STKEY,@VALUE) END ";
            Cmd.CommandText += " ELSE BEGIN UPDATE tblSetting SET [VALUE] = @VALUE WHERE STKEY = @STKEY END";
            Cmd.Parameters.Add("STKEY", SqlDbType.NVarChar).Value = key;
            Cmd.Parameters.Add("VALUE", SqlDbType.NVarChar).Value = value;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(cachekey);
            Cmd.Close();

            if (!ret) throw new Exception();
        }

    }
}
