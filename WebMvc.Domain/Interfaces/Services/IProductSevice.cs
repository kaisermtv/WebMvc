﻿using System;
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

        void Set(Product product, ProductAttribute attribute, string value);

        void Update(ProductClass cat);
        void Update(Product topic);

        void Del(Product product);

        void DelAllAttributeForProductClass(Guid guid);

        ProductClass GetProductClass(Guid Id);

        Product Get(Guid Id);
        Product GetBySlug(string Slug);
        ProductAttributeValue GetAttributeValue(Guid productid, Guid attributeid);
        int GetCount(Category cat);
        List<Product> GetList(Guid Id, int limit = 10, int page = 1);
        List<Product> GetList(Category cat, int limit = 10, int page = 1);
        int GetCount();
        List<Product> GetList(int limit = 10, int page = 1);
        List<ProductClassAttribute> GetListProductClassAttributeForProductClassId(Guid id);

        int GetCount(ProductClass productClass);
        List<Product> GetList(ProductClass productClass, int limit = 10, int page = 1);

        List<ProductAttribute> GetAllAttribute();
        ProductAttribute GetAttribute(Guid id);
        ProductAttribute GetAttribute(string name);
        void Update(ProductAttribute cat);
        List<ProductClass> GetAllProductClass();

        int GetCount(string seach);
        List<Product> GetList(string seach, int limit = 10, int page = 1);
    }
}
