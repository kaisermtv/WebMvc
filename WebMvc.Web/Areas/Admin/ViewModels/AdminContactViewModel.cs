using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Domain.DomainModel.Entities;
using System.ComponentModel.DataAnnotations;
using WebMvc.Web.Application;
using System.Web.Mvc;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class AdminContactViewModel
    {
        public List<Contact> ListContact { get; set; }
    }

    public class AdminContactEditViewModel
    {
        [Required]
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Contact.Name")]
        public string Name { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Contact.Email")]
        public string Email { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Contact.Content")]
        public string Content { get; set; }

        [WebMvcResourceDisplayName("Contact.IsCheck")]
        public bool IsCheck { get; set; }

        [WebMvcResourceDisplayName("Contact.Note")]
        public string Note { get; set; }
    }
}