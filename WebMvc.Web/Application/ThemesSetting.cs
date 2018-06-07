using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMvc.Domain.Interfaces.Services;

namespace WebMvc.Web.Application
{
    public class ThemesSetting
    {
        #region Cache
        private static Hashtable cache = new Hashtable();

        public static void ClearCahe()
        {
            cache.Clear();
        }
        #endregion

        public static object getValue(string value)
        {
            var theme = DependencyResolver.Current.GetService<ISettingsService>().GetSetting("Theme").ToLower();
            Hashtable buff = null;

            try
            {
                if (cache.ContainsKey(theme))
                {
                    buff = (Hashtable)cache[theme];
                }
            }
            catch { }


            if (buff == null)
            {
                var fp = HostingEnvironment.MapPath(string.Concat("~/Themes/", theme, "/config.json"));
                var str = File.ReadAllText(fp);

                buff = new Hashtable();

                foreach (var item in (dynamic)JsonConvert.DeserializeObject(str))
                {
                    buff.Add((string)item.Name, item.Value.Value);
                }

                cache[theme] = buff;
            }



            return buff[value];
        }

        public static bool setSettingTheme(string themeName, dynamic buf)
        {
            var fp = HostingEnvironment.MapPath(string.Concat("~/Themes/", themeName, "/config.json"));
            try
            {
                File.WriteAllText(fp, JsonConvert.SerializeObject(buf));

                cache[themeName.ToLower()] = null;
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static dynamic getSettingTheme(string themeName)
        {
            try
            {
                var fp = HostingEnvironment.MapPath(string.Concat("~/Themes/", themeName, "/config.json"));
                var str = File.ReadAllText(fp);

                return (dynamic)JsonConvert.DeserializeObject(str);
            }
            catch
            {
                return new ExpandoObject();
            }
        }

        public static dynamic getThemeInfo(string themeName)
        {
            dynamic ret = new ExpandoObject();

            ret.Name = themeName;

            if (File.Exists(HostingEnvironment.MapPath(string.Concat("~/Themes/", themeName, "/config.json"))))
            {
                ret.Config = true;
            }
            else
            {
                ret.Config = false;
            }

            return ret;
        }

        public static List<dynamic> getThemesInfo()
        {
            List<dynamic> list = new List<dynamic>();

            var listtheme = AppHelpers.GetThemeFolders();
            foreach (var it in listtheme)
            {
                list.Add(getThemeInfo(it));
            }

            return list;
        }




    }
}