using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Web.ViewModels
{
    public class CartViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Addren { get; set; }

        public string Ship_Name { get; set; }
        public string Ship_Phone { get; set; }
        public string Ship_Addren { get; set; }
        public string Ship_Note { get; set; }

        public int Payments { get; set; }
        public int Transport { get; set; }

        public List<CartItemViewModel> Products { get; set; }
        public Int64 TotalMoney { get; set; }
    }

    public class CartAddViewModel
    {
        public Guid Id;
    }
    
    public class CartListViewModel
    {
        public int State;
        public string Message;
        public List<CartItemViewModel> Products;
        public int Count;
        public Int64 TotalMoney;
    }

    public class CartItemViewModel
    {
        public Guid Id;
        public string name;
        public string Image;
        public Int64 Count;
        public string Price;
        public Int64 Priceint;
        public string Guarantee;
        public string link;

    }
}