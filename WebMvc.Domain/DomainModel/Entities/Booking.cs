using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class Booking : Entity
    {
        public Booking()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string NamePartner { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adukts { get; set; }
        public int Adolescent { get; set; }
        public int Children { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? TypeRoom_Id { get; set; }
        public bool IsCheck { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
