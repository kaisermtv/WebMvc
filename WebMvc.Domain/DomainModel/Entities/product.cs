using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class Product : Entity
    {
        public Product()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid ProductClassId { get; set; }
        public string Name { get; set; }
        public string ShotContent { get; set; }
        public bool isAutoShotContent { get; set; }
        public string Image { get; set; }
        public DateTime CreateDate { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public bool IsLocked { get; set; }

        public Guid? MembershipUser_Id { get; set; }
        public Guid? Category_Id { get; set; }
        public Guid? ProductPost_Id { get; set; }

    }
}
