using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;

namespace WebMvc.Services
{
    public partial class BookingSevice : IBookingSevice
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public BookingSevice(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private Booking DataRowToBooking(DataRow data)
        {
            if (data == null) return null;

            Booking booking = new Booking();

            booking.Id = new Guid(data["Id"].ToString());
            booking.NamePartner = data["NamePartner"].ToString();
            booking.Email = data["Email"].ToString();
            booking.CheckIn = (DateTime)data["CheckIn"];
            booking.CheckOut = (DateTime)data["CheckOut"];
            booking.Adukts = (int)data["Adukts"];
            booking.Adolescent = (int)data["Adolescent"];
            booking.Children = (int)data["Children"];
            booking.Phone = (string)data["Phone"];
            if (!data["TypeRoom_Id"].ToString().IsNullEmpty()) booking.TypeRoom_Id = new Guid(data["TypeRoom_Id"].ToString());
            booking.IsCheck = (bool)data["IsCheck"];
            booking.CreateDate = (DateTime)data["CreateDate"];
            booking.Note = data["Note"].ToString();

            return booking;
        }
        #endregion

        public void Add(Booking booking)
        {
            //string cachekey = string.Concat(CacheKeys.Category.StartsWith, "getSetting-", key);

            var Cmd = _context.CreateCommand();

            booking.CreateDate = DateTime.UtcNow;

            Cmd.CommandText = "INSERT INTO [Booking]([Id],[NamePartner],[CheckIn],[CheckOut],[Adukts],[Adolescent],[Children],[TypeRoom_Id],[Phone],[Email],[IsCheck],[Note],[CreateDate])"
                + " VALUES(@Id,@NamePartner,@CheckIn,@CheckOut,@Adukts,@Adolescent,@Children,@TypeRoom_Id,@Phone,@Email,@IsCheck,@Note,@CreateDate)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = booking.Id;
            Cmd.AddParameters("NamePartner", booking.NamePartner);
            Cmd.AddParameters("CheckIn", booking.CheckIn);
            Cmd.AddParameters("CheckOut", booking.CheckOut);
            Cmd.AddParameters("Adukts", booking.Adukts);
            Cmd.AddParameters("Adolescent", booking.Adolescent);
            Cmd.AddParameters("Children", booking.Children);
            Cmd.AddParameters("TypeRoom_Id", booking.TypeRoom_Id);
            Cmd.AddParameters("IsCheck", booking.IsCheck);
            Cmd.AddParameters("Phone", booking.Phone);
            Cmd.AddParameters("Note", booking.Note);
            Cmd.AddParameters("Email", booking.Email);
            Cmd.AddParameters("CreateDate", booking.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Booking.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Booking false");
        }


        public Booking Get(Guid Id)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "SELECT * FROM [Booking] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

            DataRow data = Cmd.findFirst();
            if (data == null) return null;

            return DataRowToBooking(data);
        }


        public void Del(Booking emp)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Booking] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = emp.Id;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Booking.StartsWith);
            Cmd.Close();

        }

        public List<Booking> GetList(int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Booking]) AS MyDerivedTable WHERE RowNum > @Offset";

            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            List<Booking> rt = new List<Booking>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToBooking(it));
            }

            return rt;
        }
    }
}
