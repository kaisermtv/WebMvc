using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebMvc.Web.Application;

namespace WebMvc.Web.ViewModels
{
    public class MemberViewModel
    {
    }

    public class LogOnViewModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Members.Label.Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [WebMvcResourceDisplayName("Members.Label.Password")]
        public string Password { get; set; }

        [WebMvcResourceDisplayName("Members.Label.RememberMe")]
        public bool RememberMe { get; set; }
    }
    public class MemberAddViewModel
    {
        [Required]
        [WebMvcResourceDisplayName("Members.Label.Username")]
        [StringLength(150, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [WebMvcResourceDisplayName("Members.Label.EmailAddress")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [WebMvcResourceDisplayName("Members.Label.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        [WebMvcResourceDisplayName("Members.Label.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int MinPasswordLength { get; set; }

        [WebMvcResourceDisplayName("Members.Label.UserIsApproved")]
        public bool IsApproved { get; set; }

        [WebMvcResourceDisplayName("Members.Label.Comment")]
        public string Comment { get; set; }

        [WebMvcResourceDisplayName("Members.Label.Roles")]
        public string[] Roles { get; set; }

        //public IList<MembershipRole> AllRoles { get; set; }
        public string SpamAnswer { get; set; }
        public string ReturnUrl { get; set; }
        public string SocialProfileImageUrl { get; set; }
        public string UserAccessToken { get; set; }
        //public LoginType LoginType { get; set; }
    }
}