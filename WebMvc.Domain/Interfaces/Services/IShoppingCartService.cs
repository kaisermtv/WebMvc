using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IShoppingCartService
    {
        ShoppingCart Get(Guid Id);
        int GetCount();
        List<ShoppingCart> GetList(int limit = 10, int page = 1);
    }
}
