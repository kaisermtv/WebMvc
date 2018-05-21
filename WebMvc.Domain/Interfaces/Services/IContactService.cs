using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IContactService
    {
        void Add(Contact contact);
        Contact Get(Guid Id);
        void Update(Contact contact);
        List<Contact> GetList(int limit = 10, int page = 1);
    }
}
