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
    public class AdminMenusViewModel
    {
        public List<Menu> Menus { get; set; }
    }

    public class AdminMenuEditViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Tên menu")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }


        [DisplayName("Mô tả")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Description { get; set; }

        [DisplayName("Màu hiển thị")]
        [UIHint(AppConstants.EditorTemplateColourPicker), AllowHtml]
        public string Colour { get; set; }


        [DisplayName("Ảnh đại diện")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }


        [DisplayName("Số thứ tự")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }


        [DisplayName("Menu cha")]
        public Guid? ParentMenu { get; set; }
        public List<SelectListItem> AllMenus { get; set; }

        [DisplayName("Kiểu liên kết")]
        public int iType { get; set; }
        public List<SelectListItem> AllType { get; set; }

        [DisplayName("link liên kết")]
        public string Link { get; set; }
        
        [DisplayName("Liên kết trang")]
        public string LinkPage { get; set; }
        public List<SelectListItem> AllPage { get; set; }

        [DisplayName("Liên kết danh mục")]
        public string LinkCat { get; set; }
        public List<SelectListItem> AllCat { get; set; }

        [DisplayName("Liên kết bài viết")]
        public string LinkNews { get; set; }
        public string TitleNews { get; set; }

        [DisplayName("Liên kết sản phẩm")]
        public string LinkProduct { get; set; }
        public string TitleProduct { get; set; }


    }
}