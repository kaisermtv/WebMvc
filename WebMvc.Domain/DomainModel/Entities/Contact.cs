using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class Contact : Entity
    {
        public Contact()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Content { get; set; }
        public bool IsCheck { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
