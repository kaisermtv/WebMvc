using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.ViewModels
{
    public class EmployeesViewModel
    {
        public List<EmployeesRole> employeesRoles;
        public List<Employees> employees;
    }
}