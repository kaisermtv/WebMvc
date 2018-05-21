using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class Topic : Entity
    {
        public Topic()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShotContent { get; set; }
        public bool isAutoShotContent { get; set; }
        public string Image { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Solved { get; set; }
        public bool? SolvedReminderSent { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }

        public Guid? MembershipUser_Id { get; set; }
        public Guid? Category_Id { get; set; }
        public Guid? Post_Id { get; set; }
        public Guid? Poll_Id { get; set; }

    }
}
