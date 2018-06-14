using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IShoppingCartProductService
    {
        void Add(ShoppingCartProduct cat);
        List<ShoppingCartProduct> GetList(ShoppingCart cart);
    }
}
