using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class CustomCodeViewModels
    {
        [AllowHtml]
        [DisplayName("Custom Header Code")]
        public string CustomHeaderCode { get; set; }

        [AllowHtml]
        [DisplayName("Custom Footer Code")]
        public string CustomFooterCode { get; set; }
    }

    public class AdminGeneralSettingViewModel
    {
        [DisplayName("Website Name")]
        [Required]
        [StringLength(200)]
        public string WebsiteName { get; set; }

        [DisplayName("Website Domain")]
        [Required]
        [StringLength(200)]
        public string WebsiteUrl { get; set; }

        [DisplayName("Close Website")]
        [Description("Close the Website for maintenance")]
        public bool IsClosed { get; set; }

        [DisplayName("Allow Rss Feeds")]
        [Description("Show the RSS feed icons for the Topics and Categories")]
        public bool EnableRSSFeeds { get; set; }

        [DisplayName("Show Edited By Details On Posts")]
        public bool DisplayEditedBy { get; set; }

        [DisplayName("Allow File Attachments On Posts")]
        public bool EnablePostFileAttachments { get; set; }

        [DisplayName("Allow Posts To Be Marked As Solution")]
        public bool EnableMarkAsSolution { get; set; }

        [DisplayName("Timeframe in days to wait before a reminder email is sent to topic creator, for all topics that have not been marked as solution - Set to 0 to disable")]
        public int MarkAsSolutionReminderTimeFrame { get; set; }

        [DisplayName("Enable Spam Reporting")]
        public bool EnableSpamReporting { get; set; }

        [DisplayName("Enable Emoticons (Smilies)")]
        public bool EnableEmoticons { get; set; }

        [DisplayName("Allow Members To Report Other Members")]
        public bool EnableMemberReporting { get; set; }

        [DisplayName("Allow Email Subscriptions")]
        public bool EnableEmailSubscriptions { get; set; }

        [DisplayName("New Members Must Confirm Their Account Via A Link Sent In An Email - Will not work with Twitter accounts!")]
        public bool NewMemberEmailConfirmation { get; set; }

        [DisplayName("Manually Authorise New Members")]
        public bool ManuallyAuthoriseNewMembers { get; set; }

        [DisplayName("Email Admin On New Member Signup")]
        public bool EmailAdminOnNewMemberSignUp { get; set; }

        [DisplayName("Number Of Topics Per Page")]
        public int TopicsPerPage { get; set; }

        [DisplayName("Number Of Posts Per Page")]
        public int PostsPerPage { get; set; }

        [DisplayName("Number Of Activities Per Page")]
        public int ActivitiesPerPage { get; set; }

        [DisplayName("Allow Private Messages")]
        public bool EnablePrivateMessages { get; set; }
        
        [DisplayName("Disable Standard Registration")]
        public bool DisableStandardRegistration { get; set; }
        
        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }
    }

    public class AdminLanguageSettingViewModel
    {
        [DisplayName("Language Default")]
        public Guid? LanguageDefault { get; set; }
        public List<SelectListItem> AllLanguage { get; set; }
    }

    public class AdminTermsConditionsSettingViewModel
    {

    }
    public class AdminEmailSettingViewModel
    {

    }

    public class AdminRegistrationSettingViewModel
    {

    }
}