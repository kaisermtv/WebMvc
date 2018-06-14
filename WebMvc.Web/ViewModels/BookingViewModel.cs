using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMvc.Web.ViewModels
{
    public class BookingCreateViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public string NamePartner { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adukts { get; set; }
        public int Adolescent { get; set; }
        public int Children { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? TypeRoom_Id { get; set; }
        public bool IsCheck { get; set; }
        public string Note { get; set; }

        public List<SelectListItem> ListTypeRoom;
    }
}