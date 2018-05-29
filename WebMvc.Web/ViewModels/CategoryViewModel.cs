using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Web.Application;

namespace WebMvc.Web.ViewModels
{
    public class CategoryListViewModel
    {
        public List<Category> AllCategory;
    }

    public class CategoryTopicListViewModel
    {
        public Category Cat;
        public List<Topic> ListTopic;
        public PageingViewModel Paging;
    }

    public class CategoryProductListViewModel
    {
        public Category Cat;
        public List<Product> ListProduct;
        public PageingViewModel Paging;
    }
    
}