﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface ICategoryService
    {
        void Add(Category cat);
        void Update(Category cat);
        void Del(Category menu);
        Category Get(string id);
        Category Get(Guid id);
        Category GetBySlug(string slug);
        List<Category> GetSubCategory(Category cat);
        List<Category> GetAll();
        List<Category> GetList(Guid? paren = null);
        List<Category> GetList(bool isProduct);
        List<Category> GetList(bool isProduct,Guid ? paren );
        List<SelectListItem> GetBaseSelectListCategories(List<Category> allowedCategories);
        List<Category> GetCategoriesParenCatregori(Category cat);
        List<Category> GetAllowedCategories(Guid Role);
        List<Category> GetAllowedCategories(Guid Role, bool IsProduct);
        List<Category> GetAllowedEditCategories(Guid Role);
        List<Category> GetAllowedEditCategories(Guid Role, bool IsProduct);

    }
}
