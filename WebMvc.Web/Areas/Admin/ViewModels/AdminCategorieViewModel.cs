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
    public class ListCategoriesViewModel
    {
        public List<Category> Categories { get; set; }
    }

    public class CategoryViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Tên danh mục")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Description { get; set; }

        [DisplayName("Màu hiển thị")]
        [UIHint(AppConstants.EditorTemplateColourPicker), AllowHtml]
        public string CategoryColour { get; set; }

        [DisplayName("Khóa danh mục")]
        public bool IsLocked { get; set; }

        [DisplayName("Moderate all topics in this Category")]
        public bool ModerateTopics { get; set; }

        [DisplayName("Moderate all posts in this Category")]
        public bool ModeratePosts { get; set; }

        [DisplayName("Số thứ tự")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }

        [DisplayName("Danh mục cha")]
        public Guid? ParentCategory { get; set; }

        public List<SelectListItem> AllCategories { get; set; }

        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("Ảnh đại diện")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }

        [DisplayName("Danh mục sản phẩm")]
        public bool IsProduct { get; set; }
    }
}