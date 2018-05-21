using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;

namespace WebMvc.Services
{
    public partial class TypeRoomSevice : ITypeRoomSevice
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public TypeRoomSevice(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }
        #region DataRowToEntity
        private TypeRoom DataRowToTypeRoom(DataRow data)
        {
            if (data == null) return null;

            TypeRoom typeRoom = new TypeRoom();

            typeRoom.Id = new Guid(data["Id"].ToString());
            typeRoom.Name = data["Name"].ToString();
            if (!data["IsShow"].ToString().IsNullEmpty()) typeRoom.IsShow = (bool)data["IsShow"];
            typeRoom.Note = data["Note"].ToString();
            typeRoom.CreateDate = (DateTime)data["CreateDate"];
            

            return typeRoom;
        }
        #endregion

        public void Add(TypeRoom typeRoom)
        {
            var Cmd = _context.CreateCommand();

            typeRoom.CreateDate = DateTime.UtcNow;

            Cmd.CommandText = "INSERT INTO [TypeRoom]([Id],[Name],[IsShow],[Note],[CreateDate])"
                + " VALUES(@Id,@Name,@IsShow,@Note,@CreateDate)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = typeRoom.Id;
            Cmd.AddParameters("Name", typeRoom.Name);
            Cmd.AddParameters("IsShow", typeRoom.IsShow);
            Cmd.AddParameters("Note", typeRoom.Note);
            Cmd.AddParameters("CreateDate", typeRoom.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.TypeRoom.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add TypeRoom false");
        }
        public TypeRoom Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.TypeRoom.StartsWith, "Get-", id);

            var cat = _cacheService.Get<TypeRoom>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (TypeRoom it in allcat)
                {
                    if (it.Id == id)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }
        public void Update(TypeRoom typeRoom)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[TypeRoom] SET [Name] = @Name,[IsShow] = @IsShow ,[Note] = @Note ,[CreateDate] = @CreateDate WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = typeRoom.Id;
            Cmd.AddParameters("Name", typeRoom.Name);
            Cmd.AddParameters("IsShow", typeRoom.IsShow);
            Cmd.AddParameters("Note", typeRoom.Note);
            Cmd.AddParameters("CreateDate", typeRoom.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.TypeRoom.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add TypeRoom false");
        }


        public List<TypeRoom> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.TypeRoom.StartsWith, "GetAll");

            var list = _cacheService.Get<List<TypeRoom>>(cachekey);
            if (list == null)
            {
                var Cmd = _context.CreateCommand();


                Cmd.CommandText = "SELECT * FROM  [TypeRoom] ORDER BY Name DESC";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                list = new List<TypeRoom>();
                foreach (DataRow it in data.Rows)
                {
                    list.Add(DataRowToTypeRoom(it));
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }

            return list;
        }

        public List<TypeRoom> GetList(bool isShow)
        {
            string cachekey = string.Concat(CacheKeys.TypeRoom.StartsWith, "GetList-", isShow);

            var list = _cacheService.Get<List<TypeRoom>>(cachekey);
            if (list == null)
            {
                var Cmd = _context.CreateCommand();


                Cmd.CommandText = "SELECT * FROM  [dbo].[TypeRoom] WHERE [IsShow] = @IsShow ORDER BY Name DESC";
                Cmd.AddParameters("IsShow", isShow);

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                list = new List<TypeRoom>();
                foreach (DataRow it in data.Rows)
                {
                    list.Add(DataRowToTypeRoom(it));
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }

            return list;
        }


        public List<SelectListItem> GetBaseSelectListTypeRooms(List<TypeRoom> allowedTypeRoom)
        {
            var cacheKey = string.Concat(CacheKeys.TypeRoom.StartsWith, "GetBaseSelectListTypeRooms", "-", allowedTypeRoom.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedTypeRoom)
                {
                    cats.Add(new SelectListItem { Text = cat.Name, Value = cat.Id.ToString() });
                }
                return cats;
            });
        }

    }
}
