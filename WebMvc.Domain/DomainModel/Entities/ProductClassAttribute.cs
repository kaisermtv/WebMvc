using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class ProductClassAttribute : Entity
    {
        public ProductClassAttribute()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid ProductClassId { get; set; }
        public Guid ProductAttributeId { get; set; }
        public bool IsShow { get; set; }
    }
}
