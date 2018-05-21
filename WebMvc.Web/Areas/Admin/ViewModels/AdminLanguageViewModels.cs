using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class LanguagesHomeViewModel
    {
        public LanguageImportExportViewModel LanguageViewModel { get; set; }
    }

    public class LanguageImportExportViewModel
    {
        public IEnumerable<CultureInfo> ExportLanguages { get; set; }
        public IEnumerable<CultureInfo> ImportLanguages { get; set; }
    }

    public class AjaxEditLanguageValueViewModel
    {
        [Required]
        public Guid LanguageId { get; set; }

        [Required]
        public string ResourceKey { get; set; }

        [Required]
        public string NewValue { get; set; }
    }

    public class ResourceKeyListViewModel
    {
        [Required]
        [Display(Name = "Resources")]
        public List<LocaleResourceKeyViewModel> ResourceKeys { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
    }

    public class LocaleResourceKeyViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        [DisplayName("Date Added")]
        public DateTime DateAdded { get; set; }
    }

    /// <summary>
    /// Creation of a new language
    /// </summary>
    public class CreateLanguageViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Language Name")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }

    public class ListLanguagesViewModel
    {

        public class LanguageDisplayViewModel
        {
            [HiddenInput]
            public Guid Id { get; set; }

            [HiddenInput]
            public bool IsDefault { get; set; }

            [DisplayName("Name")]
            [Required]
            public string Name { get; set; }

            [DisplayName("Language and Culture")]
            [Required]
            public string LanguageCulture { get; set; }
        }

        public List<LanguageDisplayViewModel> Languages { get; set; }
    }


    public class LocaleResourceViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid ResourceKeyId { get; set; }

        [DisplayName("Name")]
        public string LocaleResourceKey { get; set; }

        [DisplayName("Value")]
        public string ResourceValue { get; set; }
    }


    public class LanguageListResourcesViewModel
    {
        [Required]
        [Display(Name = "Resources")]
        public List<LocaleResourceViewModel> LocaleResources { get; set; }

        [HiddenInput]
        public Guid LanguageId { get; set; }

        public string LanguageName { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
    }
}