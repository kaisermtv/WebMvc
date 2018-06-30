using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IProductPostSevice
    {
        void Add(ProductPost post);
        ProductPost Get(Guid Id);
        void Update(ProductPost post);
        void Del(ProductPost post);
        void Del(Product product);
        void DelByProduct(Guid Id);
    }
}
