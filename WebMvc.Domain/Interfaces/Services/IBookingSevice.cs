using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IBookingSevice
    {
        void Add(Booking booking);
        Booking Get(Guid Id);
        void Del(Booking emp);
        List<Booking> GetList(int limit = 10, int page = 1);
    }
}
