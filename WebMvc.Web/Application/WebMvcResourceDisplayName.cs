using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;

namespace WebMvc.Web.Application
{
    public class WebMvcResourceDisplayName : DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;
        private readonly ILocalizationService _localizationService;

        public WebMvcResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _localizationService = ServiceFactory.Get<ILocalizationService>();
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                _resourceValue = _localizationService.GetResourceString(ResourceKey.Trim());
                
                return _resourceValue;
            }
        }

        public string Name
        {
            get { return "WMVCResourceDisplayName"; }
        }
    }
}