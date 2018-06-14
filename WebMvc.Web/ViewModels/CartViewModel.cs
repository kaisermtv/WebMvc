using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMvc.Web.ViewModels
{
    public class CartListProductViewModel
    {
        public List<CartAddViewModel> Products { get; set; }
    }

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
        [HiddenInput]
        public Guid Id { get; set; }
        public int Count { get; set; }
    }
    
    public class CartListViewModel
    {
        public int State { get; set; }
        public string Message { get; set; }
        public List<CartItemViewModel> Products { get; set; }
        public int Count { get; set; }
        public Int64 TotalMoney { get; set; }
    }

    public class CartItemViewModel
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public string Image { get; set; }
        public Int64 Count { get; set; }
        public string Price { get; set; }
        public Int64 Priceint { get; set; }
        public string Guarantee { get; set; }
        public string link { get; set; }

    }
}