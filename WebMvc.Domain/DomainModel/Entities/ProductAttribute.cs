using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class ProductAttribute : Entity
    {
        public ProductAttribute()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string LangName { get; set; }
        public int ValueType { get; set; }
        public bool IsNull { get; set; }
        public bool IsLock { get; set; }
    }
}
