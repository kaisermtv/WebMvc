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
}