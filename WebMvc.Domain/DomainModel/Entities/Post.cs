using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class Post : Entity
    {
        public Post()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string PostContent { get; set; }
        public DateTime DateCreated { get; set; }
        public int VoteCount { get; set; }
        public DateTime DateEdited { get; set; }
        public bool IsSolution { get; set; }
        public bool IsTopicStarter { get; set; }
        public bool? FlaggedAsSpam { get; set; }
        public string IpAddress { get; set; }
        public bool? Pending { get; set; }
        public string SearchField { get; set; }
        public Guid? InReplyTo { get; set; }
        public Guid Topic_Id { get; set; }
        public Guid MembershipUser_Id { get; set; }
    }
}
