using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebMvc.Web.Application;

namespace WebMvc.Web.ViewModels
{
    public class ContactCreateViewModel
    {
        [Required]
        [WebMvcResourceDisplayName("Contact.Name")]
        public string Name { get; set; }
        
        [Required]
        [WebMvcResourceDisplayName("Contact.Phone")]
        public string Phone { get; set; }

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

    public class ContactReturnJson {
        public int Status { get; set; }
        public string Message { get; set; }
    }
}