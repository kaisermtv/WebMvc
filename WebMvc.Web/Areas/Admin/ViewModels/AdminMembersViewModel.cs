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
}   