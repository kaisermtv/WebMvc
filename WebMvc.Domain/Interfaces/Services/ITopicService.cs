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

        Topic GetBySlug(string Slug);

        int GetCount();
        int GetCount(Guid Id);
        List<Topic> GetList(int limit = 10, int page = 1);
        List<Topic> GetList(Guid Id, int limit = 10, int page = 1);
    }
}
