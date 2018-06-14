using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IMenuService
    {
        List<Menu> GetAll();
        void Add(Menu menu);
        void Update(Menu menu);
        Menu Get(string id);
        Menu Get(Guid id);

        List<Menu> GetMenusParenMenu(Menu cat);

        List<SelectListItem> GetBaseSelectListMenus(List<Menu> allowedCategories);
    }
}
