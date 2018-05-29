using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class AdminBookingListViewModel
    {
        public List<Booking> ListBooking;
    }

    public class AdminTypeRoomListViewModel
    {
        public List<TypeRoom> ListTypeRoom;
    }

    public class AdminTypeRoomEditViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsShow { get; set; }

        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Note { get; set; }
    }
}