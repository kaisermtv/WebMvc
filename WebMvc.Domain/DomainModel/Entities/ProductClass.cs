using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class ProductClass : Entity
    {
        public ProductClass()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; }
        public string Slug { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
