using System;
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
        Category Get(string id);
        Category Get(Guid id);
        Category GetBySlug(string slug);
        List<Category> GetAll();
        List<Category> GetList(Guid? paren = null);
        List<SelectListItem> GetBaseSelectListCategories(List<Category> allowedCategories);
        List<Category> GetCategoriesParenCatregori(Category cat);
        List<Category> GetAllowedCategories(Guid Role);
        List<Category> GetAllowedEditCategories(Guid Role);
    }
}
