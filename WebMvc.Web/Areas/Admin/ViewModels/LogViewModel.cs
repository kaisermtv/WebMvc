using System;
using System.Collections.Generic;
using WebMvc.Domain.DomainModel.General;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class ListLogViewModel
    {
        public IList<LogEntry> LogFiles { get; set; }
    }
}