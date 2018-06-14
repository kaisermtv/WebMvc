using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Web.ViewModels
{
    public class AjaxShowroomViewModel
    {
        public List<AjaxShowroomItemViewModel> Showrooms { get; set; }
    }

    public class AjaxShowroomItemViewModel
    {
        public string Addren { get; set; }
        public string iFrameMap { get; set; }
    }
}