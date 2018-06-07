using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class Employees : Entity
    {
        public Employees()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
    }
}
