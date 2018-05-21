using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Linq;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;
using WebMvc.Domain.DomainModel.General;
using System.Text;
using System.Web.Mvc;

namespace WebMvc.Services
{
    public partial class LocalizationService : ILocalizationService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILoggingService _loggingService;
        private readonly ISettingsService _settingsService;
        private Language _currentLanguage;
        
        public Guid Lang_Id
        {
            get
            {
                if (_currentLanguage == null)
                {
                    _currentLanguage = CurrentLanguage;
                }
                return _currentLanguage.Id;
            }
        }

        public LocalizationService(IWebMvcContext context, ICacheService cacheService, ILoggingService loggingService, ISettingsService settingsService)
        {
            _context = context as WebMvcContext;
            _cacheService = cacheService;
            _settingsService = settingsService;
            _loggingService = loggingService;
        }


        #region DataRowToEntity
        private Language DataRowToLanguage(DataRow data)
        {
            if (data == null) return null;

            Language cat = new Language();

            cat.Id = new Guid(data["Id"].ToString());
            cat.Name = data["Name"].ToString();
            cat.LanguageCulture = data["LanguageCulture"].ToString();
            cat.FlagImageFileName = data["FlagImageFileName"].ToString();
            cat.RightToLeft = (bool)data["RightToLeft"];


            return cat;
        }

        private LocaleResourceKey DataRowToLocaleResourceKey(DataRow data)
        {
            if (data == null) return null;

            var cat = new LocaleResourceKey();

            cat.Id = new Guid(data["Id"].ToString());
            cat.Name = data["Name"].ToString();
            cat.Notes = data["Notes"].ToString();
            cat.DateAdded = (DateTime)data["DateAdded"];
            
            return cat;
        }

        private LocaleStringResource DataRowToLocaleStringResource(DataRow data)
        {
            if (data == null) return null;

            var cat = new LocaleStringResource();

            cat.Id = new Guid(data["Id"].ToString());
            cat.ResourceValue = data["ResourceValue"].ToString();
            cat.LocaleResourceKey_Id = new Guid(data["LocaleResourceKey_Id"].ToString());
            cat.Language_Id = new Guid(data["Language_Id"].ToString());

            return cat;
        }
        #endregion

        public Language Get(string id)
        {
            return Get(new Guid(id));
        }
        public Language Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "Get-", id);

            var cat = _cacheService.Get<Language>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (Language it in allcat)
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

        public Language Add(CultureInfo cultureInfo)
        {
            // Create a domain language object
            var language = new Language
            {
                Name = cultureInfo.EnglishName,
                LanguageCulture = cultureInfo.Name,
            };

            Add(language);

            return language;
        }
        
        
        public void Add(Language language)
        {
            var existingLanguage = GetLanguageByLanguageCulture(language.LanguageCulture);

            if (existingLanguage != null)
            {
                throw new LanguageOrCultureAlreadyExistsException(
                    $"There is already a language defined for language-culture '{existingLanguage.LanguageCulture}'");
            }

            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [dbo].[Language]([Id],[Name],[LanguageCulture],[FlagImageFileName],[RightToLeft])"
                + " VALUES(@Id,@Name,@LanguageCulture,@FlagImageFileName,@RightToLeft)";


            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = language.Id;
            Cmd.AddParameters("Name", language.Name);
            Cmd.AddParameters("LanguageCulture", language.LanguageCulture);
            Cmd.AddParameters("FlagImageFileName", language.FlagImageFileName);
            Cmd.AddParameters("RightToLeft", language.RightToLeft);


            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Localization.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Language false");

        }

        public void Add(LocaleResourceKey localeResourceKey)
        {
            localeResourceKey.DateAdded = DateTime.UtcNow;
            
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [dbo].[LocaleResourceKey]([Id],[Name],[Notes],[DateAdded])"
                + " VALUES(@Id,@Name,@Notes,@DateAdded)";
            
            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = localeResourceKey.Id;
            Cmd.AddParameters("Name", localeResourceKey.Name);
            Cmd.AddParameters("Notes", localeResourceKey.Notes);
            Cmd.AddParameters("DateAdded", localeResourceKey.DateAdded);
            
            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Localization.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add LocaleResourceKey false");

        }


        public void Add(LocaleStringResource localeStringResource)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [dbo].[LocaleStringResource]([Id],[ResourceValue],[LocaleResourceKey_Id],[Language_Id])"
                + " VALUES(@Id,@ResourceValue,@LocaleResourceKey_Id,@Language_Id)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = localeStringResource.Id;
            Cmd.AddParameters("ResourceValue", localeStringResource.ResourceValue);
            Cmd.AddParameters("LocaleResourceKey_Id", localeStringResource.LocaleResourceKey_Id);
            Cmd.AddParameters("Language_Id", localeStringResource.Language_Id);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Localization.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add LocaleStringResource false");

        }

        public void Update(LocaleStringResource localeStringResource)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[LocaleStringResource] SET [ResourceValue] = @ResourceValue,[LocaleResourceKey_Id] = @LocaleResourceKey_Id,[Language_Id] = @Language_Id WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = localeStringResource.Id;
            Cmd.AddParameters("ResourceValue", localeStringResource.ResourceValue);
            Cmd.AddParameters("LocaleResourceKey_Id", localeStringResource.LocaleResourceKey_Id);
            Cmd.AddParameters("Language_Id", localeStringResource.Language_Id);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Localization.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update LocaleStringResource false");

        }

        /// <summary>
        /// Retrieve a language by the language-culture string e.g. "en-GB"
        /// </summary>
        /// <param name="languageCulture"></param>
        public Language GetLanguageByLanguageCulture(string languageCulture)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetLanguageByLanguageCulture-", languageCulture);

            var cat = _cacheService.Get<Language>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (Language it in allcat)
                {
                    if (it.LanguageCulture == languageCulture)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }


        /// <summary>
        /// Update a resource string
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="resourceKey"></param>
        /// <param name="newValue"></param>
        public void UpdateResourceString(Guid languageId, string resourceKey, string newValue)
        {
            newValue = StringUtils.SafePlainText(newValue);

            // Get the resource
            var localeStringResource = GetResource(languageId, resourceKey);

            if (localeStringResource == null)
            {
                var objResourceKey = GetResourceKey(resourceKey);

                if (objResourceKey == null)
                {
                    throw new ApplicationException(
                        $"Unable to update resource with key {resourceKey} for language {languageId}. No resource found.");
                }

                localeStringResource = new LocaleStringResource
                {
                    Language_Id = languageId,
                    LocaleResourceKey_Id = objResourceKey.Id,
                    ResourceValue = newValue
                };

                Add(localeStringResource);
            }
            else
            {
                localeStringResource.ResourceValue = newValue;


                Update(localeStringResource);
            }
        }

        public List<SelectListItem> GetBaseSelectListLanguages(List<Language> allowedLanguages)
        {
            var cacheKey = string.Concat(CacheKeys.Localization.StartsWith, "GetBaseSelectListLanguages", "-", allowedLanguages.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedLanguages)
                {
                    cats.Add(new SelectListItem { Text = cat.Name, Value = cat.Id.ToString() });
                }
                return cats;
            });
        }

        public List<Language> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetAll");

            var allCat = _cacheService.Get<List<Language>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [Language] ORDER BY [Name] DESC";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<Language>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToLanguage(it));
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);
            }
            return allCat;
        }
        
        public IEnumerable<Language> AllLanguages => GetAll();

        public IList<CultureInfo> LanguagesInDb
        {
            get
            {
                return AllLanguages.Select(language => LanguageUtils.GetCulture(language.LanguageCulture)).OrderBy(info => info.EnglishName).ToList();
            }
        }

        public IList<CultureInfo> LanguagesAll
        {
            get
            {
                var allLanguagesNotInDb = new List<CultureInfo>();

                foreach (var cultureInfo in LanguageUtils.AllCultures)
                {
                    allLanguagesNotInDb.Add(cultureInfo);
                }

                return allLanguagesNotInDb.OrderBy(info => info.EnglishName).ToList();
            }
        }

        /// <summary>
        /// Get all the localization values (cached)
        /// </summary>
        /// <returns></returns>
        public Language CurrentLanguage
        {
            get
            {
                try
                {
                    if (HttpContext.Current != null)
                    {
                        // Check for cookie, as the user may have switched the language from the deafult one
                        var languageCooke = HttpContext.Current.Request.Cookies[AppConstants.LanguageIdCookieName];
                        if (languageCooke != null)
                        {
                            // See if it's the same language as already set
                            var languageGuid = new Guid(languageCooke.Value);
                            if (_currentLanguage != null && languageGuid == _currentLanguage.Id)
                            {
                                return _currentLanguage;
                            }

                            // User might have a language set
                            var changedLanguage = Get(languageGuid);
                            if (changedLanguage != null)
                            {
                                // User has set the language so overide it here
                                _currentLanguage = changedLanguage;
                            }
                        }
                    }

                }
                catch
                {
                    // App Start cause this to error
                    // http://stackoverflow.com/questions/2518057/request-is-not-available-in-this-context
                }

                return _currentLanguage ?? (_currentLanguage = DefaultLanguage);
            }

            set { _currentLanguage = value; }
        }

        /// <summary>
        /// The system default language
        /// </summary>
        public Language DefaultLanguage
        {
            get
            {
                var language_Id = _settingsService.GetSetting("LanguageDefault");

                if (language_Id.IsNullEmpty())
                {
                    // This is a one off scenario and means the system has no settings
                    // usually when running the installer, so we need to return a default language
                    return new Language { Name = "Setup Language", LanguageCulture = "en-GB" };
                }
                
                // If we get here just set the default language
                var language = Get(language_Id);

                if (language == null)
                {
                    throw new ApplicationException("There is no default language defined in the system.");
                }

                return language;
            }
        }

        /// <summary>
        /// Import a language from CSV
        /// </summary>
        /// <param name="langKey"> </param>
        /// <param name="allLines"></param>
        /// <returns>A report on the import</returns>
        public CsvReport FromCsv(string langKey, List<string> allLines)
        {
            var report = new CsvReport();

            if (allLines == null || allLines.Count == 0)
            {
                report.Errors.Add(new CsvErrorWarning
                {
                    ErrorWarningType = CsvErrorWarningType.BadDataFormat,
                    Message = "No language keys or values found."
                });
                return report;
            }

            // Look up the language and culture
            Language language;

            try
            {
                var cultureInfo = LanguageUtils.GetCulture(langKey);

                if (cultureInfo == null)
                {
                    report.Errors.Add(new CsvErrorWarning
                    {
                        ErrorWarningType = CsvErrorWarningType.DoesNotExist,
                        Message = $"The language culture '{langKey}' does not exist."
                    });

                    return report;
                }

                // See if this language exists already, and if not then create it
                language = GetLanguageByLanguageCulture(langKey) ?? Add(cultureInfo);
            }
            catch (LanguageOrCultureAlreadyExistsException ex)
            {
                report.Errors.Add(new CsvErrorWarning { ErrorWarningType = CsvErrorWarningType.AlreadyExists, Message = ex.Message });
                return report;
            }
            catch (Exception ex)
            {
                report.Errors.Add(new CsvErrorWarning { ErrorWarningType = CsvErrorWarningType.ItemBad, Message = ex.Message });
                return report;
            }

            return FromCsv(language, allLines);
        }

        public IList<LocaleStringResource> AllLanguageResources(Guid languageId)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "AllLanguageResources-", languageId);

            var cachedSettings = _cacheService.Get<List<LocaleStringResource>>(cachekey);
            if (cachedSettings == null)
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [LocaleStringResource] WHERE [Language_Id] = @Language_Id";
                
                Cmd.Parameters.Add("Language_Id", SqlDbType.UniqueIdentifier).Value = languageId;

                DataTable data = Cmd.findAll();

                Cmd.Close();

                if (data == null) return null;

                cachedSettings = new List<LocaleStringResource>();
                foreach (DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToLocaleStringResource(it));
                }
                
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }
            return cachedSettings;
        }

        /// <summary>
        /// Convert a language into CSV format (e.g. for export)
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string ToCsv(Language language)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "SELECT ST.[ResourceValue],KY.[Name] FROM [LocaleStringResource] AS ST"
                    + " INNER JOIN [LocaleResourceKey] AS KY ON ST.LocaleResourceKey_Id = KY.Id"
                    + "  WHERE ST.[Language_Id] = @Language_Id";

            Cmd.Parameters.Add("Language_Id", SqlDbType.UniqueIdentifier).Value = language.Id;

            DataTable data = Cmd.findAll();

            Cmd.Close();

            var csv = new StringBuilder();
            foreach (DataRow resource in data.Rows)
            {
                csv.AppendFormat("{0},{1}", resource["Name"].ToString(), resource["ResourceValue"].ToString());
                csv.AppendLine();
            }

            return csv.ToString();
        }

        public CsvReport FromCsv(Language language, List<string> allLines)
        {
            var commaSeparator = new[] { ',' };
            var report = new CsvReport();

            if (allLines == null || allLines.Count == 0)
            {
                report.Errors.Add(new CsvErrorWarning
                {
                    ErrorWarningType = CsvErrorWarningType.BadDataFormat,
                    Message = "No language keys or values found."
                });
                return report;
            }

            try
            {
                //var allResourceKeys = GetAllResourceKeys();
                var lineCounter = 0;
                foreach (var line in allLines)
                {
                    lineCounter++;

                    //var keyValuePair = line.Split(commaSeparator);
                    var keyValuePair = line.Split(commaSeparator, 2, StringSplitOptions.None);

                    if (keyValuePair.Length < 2)
                    {
                        report.Errors.Add(new CsvErrorWarning
                        {
                            ErrorWarningType = CsvErrorWarningType.MissingKeyOrValue,
                            Message = $"Line {lineCounter}: a key and a value are required."
                        });

                        continue;
                    }

                    var key = keyValuePair[0];

                    if (string.IsNullOrEmpty(key))
                    {
                        // Ignore empty keys
                        continue;
                    }
                    key = key.Trim();

                    var value = keyValuePair[1];

                    var resourceKey = GetResourceKey(key);

                    if (language == null)
                    {
                        throw new ApplicationException("Unable to create language");
                    }

                    // If key does not exist, it is a new one to be created
                    if (resourceKey == null)
                    {
                        resourceKey = new LocaleResourceKey
                        {
                            Name = key,
                            DateAdded = DateTime.UtcNow,
                        };

                        Add(resourceKey);
                        report.Warnings.Add(new CsvErrorWarning
                        {
                            ErrorWarningType = CsvErrorWarningType.NewKeyCreated,
                            Message =
                                $"A new key named '{key}' has been created, and will require a value in all languages."
                        });
                    }

                    // In the new language (only) set the value for the resource
                    var stringResource = GetResource(language.Id, resourceKey.Name);
                    if (stringResource != null)
                    {
                        if (!stringResource.ResourceValue.Equals(value))
                        {
                            stringResource.ResourceValue = value;
                        }

                        Update(stringResource);
                    }
                    else
                    {
                        // No string resources have been created, so most probably
                        // this is the installer creating the keys so we need to create the 
                        // string resource to go with it and add it
                        stringResource = new LocaleStringResource
                        {
                            Language_Id = language.Id,
                            LocaleResourceKey_Id = resourceKey.Id,
                            ResourceValue = value
                        };
                        Add(stringResource);
                    }
                }
            }
            catch (Exception ex)
            {
                report.Errors.Add(new CsvErrorWarning { ErrorWarningType = CsvErrorWarningType.GeneralError, Message = ex.Message });
            }
            
            return report;
        }

        public int GetCountResourceKey()
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetCountResourceKey");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = " SELECT COUNT(*) FROM  [LocaleResourceKey]";

                try
                {
                    count = (int)Cmd.command.ExecuteScalar();
                }
                catch
                {
                    return 0;
                }

                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<LocaleResourceKey> GetListResourceKey(int page = 1, int limit = 30)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetListResourceKey-", page,"-", limit);
            var listKey = _cacheService.Get<List<LocaleResourceKey>>(cachekey);
            if (listKey == null)
            {
                var Cmd = _context.CreateCommand();

                if (page == 0) page = 1;

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY Name DESC)) AS RowNum FROM  [LocaleResourceKey]) AS MyDerivedTable WHERE RowNum > @Offset";

                //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                listKey = new List<LocaleResourceKey>();
                foreach (DataRow it in data.Rows)
                {
                    listKey.Add(DataRowToLocaleResourceKey(it));
                }

                _cacheService.Set(cachekey, listKey, CacheTimes.OneDay);
            }
            return listKey;
        }


        public List<LocaleResourceKey> GetListResourceKey(string seach,int page = 1, int limit = 30)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetListResourceKey-", seach, "-", page, "-", limit);
            var listKey = _cacheService.Get<List<LocaleResourceKey>>(cachekey);
            if (listKey == null)
            {
                var Cmd = _context.CreateCommand();

                if (page == 0) page = 1;

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY Name DESC)) AS RowNum FROM  [LocaleResourceKey] WHERE UPPER(RTRIM(LTRIM([Name]))) LIKE  N'%'+UPPER(RTRIM(LTRIM(@Seach)))+'%') AS MyDerivedTable WHERE RowNum > @Offset";

                //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;
                Cmd.Parameters.Add("Seach", SqlDbType.NVarChar).Value = seach;

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                listKey = new List<LocaleResourceKey>();
                foreach (DataRow it in data.Rows)
                {
                    listKey.Add(DataRowToLocaleResourceKey(it));
                }

                _cacheService.Set(cachekey, listKey, CacheTimes.OneDay);
            }
            return listKey;
        }

        public LocaleResourceKey GetResourceKey(string resourceKey)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetResourceKey-", resourceKey);
            var listKey = _cacheService.Get<LocaleResourceKey>(cachekey);
            if (listKey == null)
            {
                var Cmd = _context.CreateCommand();
               

                Cmd.CommandText = "SELECT * FROM  [LocaleResourceKey] WHERE [Name] = @Name";

                Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = resourceKey;

                DataRow data = Cmd.findFirst();
                Cmd.Close();

                if (data == null) return null;

                listKey = DataRowToLocaleResourceKey(data);

                _cacheService.Set(cachekey, listKey, CacheTimes.OneDay);
            }
            return listKey;
        }

        public LocaleStringResource GetValueResource(Guid value_Id, Guid lang_id)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetValueResource-", lang_id, '-', value_Id);

            var cachedSettings = _cacheService.Get<LocaleStringResource>(cachekey);
            if (cachedSettings == null)
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [LocaleStringResource] "
                    + "  WHERE [Language_Id] = @Language_Id AND [LocaleResourceKey_Id] = @LocaleResourceKey_Id";

                Cmd.Parameters.Add("LocaleResourceKey_Id", SqlDbType.UniqueIdentifier).Value = value_Id;
                Cmd.Parameters.Add("Language_Id", SqlDbType.UniqueIdentifier).Value = lang_id;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                cachedSettings = DataRowToLocaleStringResource(data);

                Cmd.Close();
                
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }
            return cachedSettings;
        }

        public LocaleStringResource GetResource(Guid lang_id,string resourceKey)
        {
            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetResource-", lang_id, '-', resourceKey);

            var cachedSettings = _cacheService.Get<LocaleStringResource>(cachekey);
            if (cachedSettings == null)
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [LocaleStringResource] AS ST "
                    + " INNER JOIN [LocaleResourceKey] AS KY ON ST.LocaleResourceKey_Id = KY.Id"
                    + "  WHERE ST.[Language_Id] = @Language_Id AND KY.[Name] = @KEY";

                Cmd.Parameters.Add("KEY", SqlDbType.NVarChar).Value = resourceKey;
                Cmd.Parameters.Add("Language_Id", SqlDbType.UniqueIdentifier).Value = lang_id;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                cachedSettings = DataRowToLocaleStringResource(data);

                Cmd.Close();

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }
            return cachedSettings;
        }

        public string GetResourceString(Guid languageId, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            key = key.Trim();

            string cachekey = string.Concat(CacheKeys.Localization.StartsWith, "GetResourceString-", languageId,'-', key);

            var cachedSettings = _cacheService.Get<string>(cachekey);
            if (cachedSettings == null)
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT ST.[ResourceValue] FROM [LocaleStringResource] AS ST"
                    + " INNER JOIN [LocaleResourceKey] AS KY ON ST.LocaleResourceKey_Id = KY.Id"
                    + "  WHERE KY.[Name] = @KEY AND ST.[Language_Id] = @Language_Id";

                Cmd.Parameters.Add("KEY", SqlDbType.NVarChar).Value = key;
                Cmd.Parameters.Add("Language_Id", SqlDbType.UniqueIdentifier).Value = languageId;

                DataRow data = Cmd.findFirst();

                Cmd.Close();

                if (data == null)
                {
                    cachedSettings = key;
                }
                else
                {
                    cachedSettings = data["ResourceValue"].ToString();
                }
                
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }
            return cachedSettings;
        }

        public string GetResourceString(string key)
        {
            try
            {
                return GetResourceString(Lang_Id, key);
            }
            catch
            {
                return key;
            }
            
        }
    }

    public class LanguageOrCultureAlreadyExistsException : Exception
    {
        public LanguageOrCultureAlreadyExistsException(string message)
            : base(message)
        {

        }
    }
}
