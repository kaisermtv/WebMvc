using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class AdminEmployeesViewModel
    {
        public List<Employees> ListEmployees { get; set; }
    }

    public class AdminEmployeesRoleViewModel
    {
        public List<EmployeesRole> ListEmployees { get; set; }
    }

    public class CreateEditEmployeesRoleViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("Tên chức vụ")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Thứ tự xắp xếp")]
        public int SortOrder { get; set; }
    }

    public class CreateEditEmployeesViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("Chức vụ")]
        [Required]
        public Guid RoleId { get; set; }
        public List<SelectListItem> Roles { get; set; }

        [DisplayName("Tên nhân viên")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }
        
        [DisplayName("Skype")]
        public string Skype { get; set; }
    }
}