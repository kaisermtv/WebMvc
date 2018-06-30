using System;
using System.Collections.Generic;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IEmployeesService
    {
        void Add(Employees role);
        void Update(Employees role);
        Employees Get(string id);
        Employees Get(Guid id);
        List<Employees> GetAll();
        void Del(Employees emp);
        List<Employees> GetList(EmployeesRole employeesRole);
    }
}
