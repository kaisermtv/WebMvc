using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class AdminListMembersViewModel
    {
        public List<MembershipUser> ListMembers { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class AdminCreateMembersViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsBanned { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
    }
}   