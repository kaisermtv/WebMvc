using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface ISettingsService
    {
        string GetSetting(string key);
        string GetSettingNoCache(string key);
        void SetSetting(string key, string value);
    }
}
