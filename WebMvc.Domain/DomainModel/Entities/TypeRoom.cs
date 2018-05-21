using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class TypeRoom : Entity
    {
        public TypeRoom()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsShow { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
