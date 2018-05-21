using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;

namespace WebMvc.Services
{
    public partial class ContactService : IContactService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public ContactService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private Contact DataRowToContact(DataRow data)
        {
            if (data == null) return null;

            Contact contact = new Contact();

            contact.Id = new Guid(data["Id"].ToString());
            contact.Name = data["Name"].ToString();
            contact.Email = data["Email"].ToString();
            contact.Content = data["Content"].ToString();
            contact.IsCheck = (bool)data["IsCheck"];
            contact.CreateDate = (DateTime)data["CreateDate"];
            contact.Note = data["Note"].ToString();
            
            return contact;
        }
        #endregion

        public void Add(Contact contact)
        {
            //string cachekey = string.Concat(CacheKeys.Category.StartsWith, "getSetting-", key);

            var Cmd = _context.CreateCommand();

            contact.CreateDate = DateTime.UtcNow;
            
            Cmd.CommandText = "INSERT INTO [Contact]([Id],[Name],[Email],[Content],[IsCheck],[Note],[CreateDate])"
                + " VALUES(@Id,@Name,@Email,@Content,@IsCheck,@Note,@CreateDate)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = contact.Id;
            Cmd.AddParameters("Name", contact.Name);
            Cmd.AddParameters("Email", contact.Email);
            Cmd.AddParameters("Content", contact.Content);
            Cmd.AddParameters("IsCheck", contact.IsCheck);
            Cmd.AddParameters("Note", contact.Note);
            Cmd.AddParameters("CreateDate", contact.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Contact.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Contact false");
        }

        public Contact Get(Guid Id)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "SELECT * FROM [Contact] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

            DataRow data = Cmd.findFirst();
            if (data == null) return null;

            return DataRowToContact(data);
        }

        public void Update(Contact contact)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [Contact] SET [Name] = @Name,[Email] = @Email,[Content] = @Content,[IsCheck] = @IsCheck, [Note] = @Note,[CreateDate] = @CreateDate WHERE Id = @Id";
            
            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = contact.Id;
            Cmd.AddParameters("Name", contact.Name);
            Cmd.AddParameters("Email", contact.Email);
            Cmd.AddParameters("Content", contact.Content);
            Cmd.AddParameters("IsCheck", contact.IsCheck);
            Cmd.AddParameters("Note", contact.Note);
            Cmd.AddParameters("CreateDate", contact.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Contact.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update Contact false");
        }

        public List<Contact> GetList(int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Contact]) AS MyDerivedTable WHERE RowNum > @Offset";

            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            List<Contact> rt = new List<Contact>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToContact(it));
            }

            return rt;
        }
    }
}
