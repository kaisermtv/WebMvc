using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Web.ViewModels
{
    public class PluginComputerBuildingViewModel
    {
        public List<PluginProductClassViewModel> ProductClass;

    }

    public class PluginProductClassViewModel
    {
        public Guid Id;
        public string Name;
        public Guid SelectId;
    }
}