using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Web.Areas.Admin.ViewModels
{
    public class AdminAttributeViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public Guid AttriId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int ValueType { get; set; }
        public bool IsNull { get; set; }
    }

    public class AdminEditProductViewModel
    {
        public Guid ProductClass { get; set; }
        public IList<AdminAttributeViewModel> AllAttribute { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }

        [DisplayName("Nội dung")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Content { get; set; }
        
        [DisplayName("Khóa comment")]
        public bool IsLocked { get; set; }

        [Required]
        [DisplayName("Danh mục")]
        public Guid? Category { get; set; }
        
        public List<SelectListItem> Categories { get; set; }

        [DisplayName("Topic.Label.SubscribeToTopic")]
        public bool SubscribeToTopic { get; set; }

        [DisplayName("Ảnh đại diện")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }

        // Permissions stuff
        //public CheckCreateTopicPermissions OptionalPermissions { get; set; }

        // Edit Properties
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid TopicId { get; set; }

        public bool IsTopicStarter { get; set; }

        //public TopicAttributeViewModel TopicAttribute { get; set; }

    }

    public class AdminProductViewModel
    {
        public Guid ProductClass { get; set; }
        public List<Product> ListProduct { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class AdminProductClassViewModel
    {
        public List<ProductClass> ListProductClass { get; set; }
    }

    public class AdminEditProductClassAttributeViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        public bool IsCheck { get; set; }
        public bool IsShow { get; set; }
    }

    public class AdminEditProductClassViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Tên nhóm sản phẩm")]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Khóa nhóm sản phẩm")]
        public bool IsLocked { get; set; }

        [DisplayName("Màu hiển thị")]
        public string Colour { get; set; }

        [DisplayName("Hình đại diện")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }

        public IList<AdminEditProductClassAttributeViewModel> AllAttribute { get; set; }
    }

    public class AdminProductAttributeViewModel
    {
        public List<ProductAttribute> ListProductAttribute { get; set; }
    }

    public class AdminCreateProductAttributeViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Lang Name")]
        public string LangName { get; set; }
        
        [DisplayName("Value Type")]
        public int ValueType { get; set; }

        [DisplayName("Is Null")]
        public bool IsNull { get; set; }

        public List<SelectListItem> AllValueType { get; set; }
    }
    
}