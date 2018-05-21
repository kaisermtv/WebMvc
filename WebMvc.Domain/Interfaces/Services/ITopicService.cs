using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface ITopicService
    {
        void Add(Topic topic);
        Topic Get(Guid Id);
        void Update(Topic topic);

        List<Topic> GetList(int limit = 10, int page = 1);
    }
}
