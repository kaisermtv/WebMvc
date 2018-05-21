using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.General;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface ILocalizationService
    {
        Language Add(CultureInfo cultureInfo);
        void Add(Language language);
        void Add(LocaleResourceKey localeResourceKey);

        void UpdateResourceString(Guid languageId, string resourceKey, string newValue);

        IEnumerable<Language> AllLanguages { get; }
        IList<CultureInfo> LanguagesInDb { get; }
        IList<CultureInfo> LanguagesAll { get; }

        CsvReport FromCsv(string langKey, List<string> allLines);
        string ToCsv(Language language);
        Language GetLanguageByLanguageCulture(string languageCulture);

        string GetResourceString(string key);

        List<SelectListItem> GetBaseSelectListLanguages(List<Language> allowedLanguages);
        List<Language> GetAll();
        Language Get(Guid id);
        Language CurrentLanguage { get; }
        Language DefaultLanguage { get; }
        int GetCountResourceKey();
        List<LocaleResourceKey> GetListResourceKey(int page = 1, int limit = 30);
        LocaleStringResource GetValueResource(Guid value_Id, Guid lang_id);
    }
}
