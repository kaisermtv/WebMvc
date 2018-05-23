using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IProductSevice
    {
        void Add(ProductAttribute cat);
        void Add(ProductClass cat);
        void Add(ProductClassAttribute cat);
        void Add(Product topic);
        void Add(ProductAttributeValue cat);

        void Update(ProductClass cat);

        void DelAllAttributeForProductClass(Guid guid);

        ProductClass GetProductClass(Guid Id);

        Product Get(Guid Id);
        ProductAttributeValue GetAttributeValue(Guid productid, Guid attributeid);
        List<Product> GetList(Guid Id, int limit = 10, int page = 1);

        List<ProductClassAttribute> GetListProductClassAttributeForProductClassId(Guid id);

        List<ProductAttribute> GetAllAttribute();
        ProductAttribute GetAttribute(Guid id);
        void Update(ProductAttribute cat);
        List<ProductClass> GetAllProductClass();
    }
}
