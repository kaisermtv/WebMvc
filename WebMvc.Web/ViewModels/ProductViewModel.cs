using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.ViewModels
{
    public class ProductViewModel
    {
        public Product product;
        public Category Cat;
        public ProductPost post;
    }

    public class ClassProductViewModel
    {
        public ProductClass ProductClass;
        public List<Product> ListProduct;
        public PageingViewModel Paging;
    }

    public class ProductValueViewModel
    {
        public string Name;
        public string Value;
        public bool IsShow;
    }

    public class ProductAjaxItem
    {
        public string productName;
        public string productUrl;
        public string price;
        public ProductAjaxImageItem productImage;
    }
    public class ProductAjaxImageItem
    {
        public string fullimg;
        public string medium;
    }
}