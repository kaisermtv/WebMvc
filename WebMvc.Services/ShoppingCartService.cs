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

namespace WebMvc.Services
{
    public partial class ShoppingCartService : IShoppingCartService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public ShoppingCartService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private ShoppingCart DataRowToShoppingCart(DataRow data)
        {
            if (data == null) return null;

            var cart = new ShoppingCart();

            cart.Id = new Guid(data["Id"].ToString());
            cart.Name = data["Name"].ToString();
            cart.Phone = data["Phone"].ToString();
            cart.Email = data["Email"].ToString();
            cart.Addren = data["Addren"].ToString();
            cart.ShipName = data["ShipName"].ToString();
            cart.ShipPhone = data["ShipPhone"].ToString();
            cart.ShipAddren = data["ShipAddren"].ToString();
            cart.ShipNote = data["ShipNote"].ToString();
            cart.TotalMoney = data["TotalMoney"].ToString();
            if(data["Status"] != DBNull.Value) cart.Status = (int)data["Status"];
            cart.Note = data["Note"].ToString();
            cart.CreateDate = (DateTime)data["CreateDate"];

            return cart;
        }
        #endregion


        public void Add(ShoppingCart cat)
        {
            var Cmd = _context.CreateCommand();

            cat.CreateDate = DateTime.UtcNow;
            
            Cmd.CommandText = "INSERT INTO [dbo].[ShoppingCart]([Id],[Name],[Email] ,[Phone],[Addren],[ShipName],[ShipPhone] ,[ShipAddren] ,[ShipNote],[TotalMoney],[Note],[Status],[CreateDate])"
                + " VALUES(@Id,@Name,@Email,@Phone,@Addren,@ShipName,@ShipPhone,@ShipAddren,@ShipNote,@TotalMoney,@Note,@Status,@CreateDate)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Email", cat.Email);
            Cmd.AddParameters("Phone", cat.Phone);
            Cmd.AddParameters("Addren", cat.Addren);
            Cmd.AddParameters("ShipName", cat.ShipName);
            Cmd.AddParameters("ShipPhone", cat.ShipPhone);
            Cmd.AddParameters("ShipAddren", cat.ShipAddren);
            Cmd.AddParameters("ShipNote", cat.ShipNote);
            Cmd.AddParameters("TotalMoney", cat.TotalMoney);
            Cmd.AddParameters("Note", cat.Note);
            Cmd.AddParameters("Status", cat.Status);
            Cmd.AddParameters("CreateDate", cat.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.ShoppingCart.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add ShoppingCart false");
        }


        public ShoppingCart Get(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.ShoppingCart.StartsWith, "Get-",Id);
            var cart = _cacheService.Get<ShoppingCart>(cachekey);
            if (cart == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [ShoppingCart] WHERE [Id] = @Id";
                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                cart = DataRowToShoppingCart(data);

                Cmd.Close();

                _cacheService.Set(cachekey, cart, CacheTimes.OneHour);
            }
            return cart;
        }


        public void Update(ShoppingCart cat)
        {
            var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [dbo].[ShoppingCart] SET [Name] = @Name,[Email] = @Email,[Phone] = @Phone,[Addren] = @Addren,[ShipName] = @ShipName,[ShipPhone] =@ShipPhone,[ShipAddren] = @ShipAddren,[ShipNote] = @ShipNote,[TotalMoney] = @TotalMoney,[Note] = @Note,[Status] = @Status,[CreateDate] = @CreateDate WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Email", cat.Email);
            Cmd.AddParameters("Phone", cat.Phone);
            Cmd.AddParameters("Addren", cat.Addren);
            Cmd.AddParameters("ShipName", cat.ShipName);
            Cmd.AddParameters("ShipPhone", cat.ShipPhone);
            Cmd.AddParameters("ShipAddren", cat.ShipAddren);
            Cmd.AddParameters("ShipNote", cat.ShipNote);
            Cmd.AddParameters("TotalMoney", cat.TotalMoney);
            Cmd.AddParameters("Note", cat.Note);
            Cmd.AddParameters("Status", cat.Status);
            Cmd.AddParameters("CreateDate", cat.CreateDate);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.ShoppingCart.StartsWith);
            Cmd.Close();

            if (!ret) throw new Exception("Update ShoppingCart false");
        }

        public int GetCount()
        {
            string cachekey = string.Concat(CacheKeys.ShoppingCart.StartsWith, "GetCount");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [ShoppingCart]";

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();

                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }

        public List<ShoppingCart> GetList(int limit = 10, int page = 1)
        {
            string cachekey = string.Concat(CacheKeys.ShoppingCart.StartsWith, "GetList-", limit, "-", page);
            var list = _cacheService.Get<List<ShoppingCart>>(cachekey);
            if (list == null)
            {
                var Cmd = _context.CreateCommand();

                if (page == 0) page = 1;

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [ShoppingCart]) AS MyDerivedTable WHERE RowNum > @Offset";

                //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                list = new List<ShoppingCart>();
                foreach (DataRow it in data.Rows)
                {
                    list.Add(DataRowToShoppingCart(it));
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }
    }
}
