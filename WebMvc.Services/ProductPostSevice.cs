using System;
using System.Data;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;


namespace WebMvc.Services
{
    public partial class ProductPostSevice : IProductPostSevice
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public ProductPostSevice(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private ProductPost DataRowToProductPost(DataRow data)
        {
            if (data == null) return null;

            ProductPost post = new ProductPost();

            post.Id = new Guid(data["Id"].ToString());
            post.PostContent = data["PostContent"].ToString();
            post.DateCreated = (DateTime)data["DateCreated"];
            post.VoteCount = (int)data["VoteCount"];
            post.DateEdited = (DateTime)data["DateEdited"];
            post.IsSolution = (bool)data["IsSolution"];
            if(!data["IsTopicStarter"].ToString().IsNullEmpty()) post.IsTopicStarter = (bool)data["IsTopicStarter"];
            if (!data["FlaggedAsSpam"].ToString().IsNullEmpty()) post.FlaggedAsSpam = (bool)data["FlaggedAsSpam"];
            post.IpAddress = data["IpAddress"].ToString();
            if (!data["Pending"].ToString().IsNullEmpty()) post.Pending = (bool)data["Pending"];
            post.SearchField = data["SearchField"].ToString();
            if (!data["InReplyTo"].ToString().IsNullEmpty()) post.InReplyTo = new Guid(data["InReplyTo"].ToString());
            post.Product_Id = new Guid(data["Product_Id"].ToString());
            post.MembershipUser_Id = new Guid(data["MembershipUser_Id"].ToString());


            return post;
        }
        #endregion

        public void Add(ProductPost post)
        {
            post.DateCreated = DateTime.UtcNow;
            post.DateEdited = post.DateCreated;

            var Cmd = _context.CreateCommand();
            

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [ProductPost] WHERE [Id] = @Id)";
            Cmd.CommandText += " BEGIN INSERT INTO [ProductPost]([Id],[PostContent],[DateCreated],[VoteCount],[DateEdited],[IsSolution],[IsTopicStarter],[FlaggedAsSpam],[IpAddress],[Pending],[SearchField],[InReplyTo],[Product_Id],[MembershipUser_Id])";
            Cmd.CommandText += " VALUES(@Id,@PostContent,@DateCreated,@VoteCount,@DateEdited,@IsSolution,@IsTopicStarter,@FlaggedAsSpam,@IpAddress,@Pending,@SearchField,@InReplyTo,@Product_Id,@MembershipUser_Id) END ";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = post.Id;
            Cmd.Parameters.Add("PostContent", SqlDbType.NVarChar).Value = post.PostContent;
            Cmd.Parameters.Add("DateCreated", SqlDbType.DateTime).Value = post.DateCreated;
            Cmd.Parameters.Add("VoteCount", SqlDbType.Int).Value = post.VoteCount;
            Cmd.Parameters.Add("DateEdited", SqlDbType.DateTime).Value = post.DateEdited;
            Cmd.Parameters.Add("IsSolution", SqlDbType.Bit).Value = post.IsSolution;
            Cmd.AddParameters("IsTopicStarter", post.IsTopicStarter);
            Cmd.AddParameters("FlaggedAsSpam", post.FlaggedAsSpam);
            Cmd.AddParameters("IpAddress", post.IpAddress);
            Cmd.AddParameters("Pending", post.Pending);
            Cmd.AddParameters("SearchField", post.SearchField);
            Cmd.AddParameters("InReplyTo", post.InReplyTo);
            Cmd.Parameters.Add("Product_Id", SqlDbType.UniqueIdentifier).Value = post.Product_Id;
            Cmd.Parameters.Add("MembershipUser_Id", SqlDbType.UniqueIdentifier).Value = post.MembershipUser_Id;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.cacheStartsWithToClear(CacheKeys.ProductPost.StartsWith);
            Cmd.Close();

            if (!ret) throw new Exception("Add ProductPost false");
        }

        public void Update(ProductPost post)
        {
            post.DateEdited = DateTime.UtcNow;

            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [ProductPost] SET [PostContent] = @PostContent, [DateCreated] = @DateCreated, [VoteCount] = @VoteCount, [DateEdited] = @DateEdited, [IsSolution] = @IsSolution, [IsTopicStarter] = @IsTopicStarter,"
                            + " [FlaggedAsSpam] = @FlaggedAsSpam, [IpAddress] = @IpAddress, [Pending] = @Pending, [SearchField] = @SearchField, [InReplyTo] = @InReplyTo, [Product_Id] = @Product_Id, [MembershipUser_Id] = @MembershipUser_Id "
                            + " WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = post.Id;
            Cmd.Parameters.Add("PostContent", SqlDbType.NVarChar).Value = post.PostContent;
            Cmd.Parameters.Add("DateCreated", SqlDbType.DateTime).Value = post.DateCreated;
            Cmd.Parameters.Add("VoteCount", SqlDbType.Int).Value = post.VoteCount;
            Cmd.Parameters.Add("DateEdited", SqlDbType.DateTime).Value = post.DateEdited;
            Cmd.Parameters.Add("IsSolution", SqlDbType.Bit).Value = post.IsSolution;
            Cmd.AddParameters("IsTopicStarter", post.IsTopicStarter);
            Cmd.AddParameters("FlaggedAsSpam", post.FlaggedAsSpam);
            Cmd.AddParameters("IpAddress", post.IpAddress);
            Cmd.AddParameters("Pending", post.Pending);
            Cmd.AddParameters("SearchField", post.SearchField);
            Cmd.AddParameters("InReplyTo", post.InReplyTo);
            Cmd.Parameters.Add("Product_Id", SqlDbType.UniqueIdentifier).Value = post.Product_Id;
            Cmd.Parameters.Add("MembershipUser_Id", SqlDbType.UniqueIdentifier).Value = post.MembershipUser_Id;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.ProductPost.StartsWith);
            Cmd.Close();

            if (!ret) throw new Exception("Update ProductPost false");
        }

        public ProductPost Get(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.ProductPost.StartsWith, "Get-", Id);

            var cat = _cacheService.Get<ProductPost>(cachekey);
            if (cat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM [ProductPost] WHERE Id = @Id";

                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                cat = DataRowToProductPost(data);

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }
        

    }
}
