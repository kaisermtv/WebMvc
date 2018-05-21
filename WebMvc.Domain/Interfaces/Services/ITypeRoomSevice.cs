using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface ITypeRoomSevice
    {
        void Add(TypeRoom typeRoom);
        TypeRoom Get(Guid id);
        void Update(TypeRoom typeRoom);
        List<TypeRoom> GetAll();
        List<TypeRoom> GetList(bool isShow);
        List<SelectListItem> GetBaseSelectListTypeRooms(List<TypeRoom> allowedTypeRoom);
    }
}
