using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IEmployeesRoleService
    {
        void Add(EmployeesRole role);
        void Update(EmployeesRole role);
        EmployeesRole Get(string id);
        EmployeesRole Get(Guid id);
        List<EmployeesRole> GetAll();

        List<SelectListItem> GetBaseSelectListEmployeesRole(List<EmployeesRole> allowedCategories);
    }
}
