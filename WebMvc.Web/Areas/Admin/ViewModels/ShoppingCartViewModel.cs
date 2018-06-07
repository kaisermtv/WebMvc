using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCart> shoppingCarts;
        public AdminPageingViewModel Paging;
    }

    public class ShoppingCartEditViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Addren { get; set; }
        public string ShipName { get; set; }
        public string ShipPhone { get; set; }
        public string ShipAddren { get; set; }
        public string ShipNote { get; set; }
        public string TotalMoney { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}