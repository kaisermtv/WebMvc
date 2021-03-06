﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;

namespace WebMvc.Services
{
    public partial class ProductSevice : IProductSevice
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public ProductSevice(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }


        #region DataRowToEntity
        private Product DataRowToProduct(DataRow data)
        {
            if (data == null) return null;

            var topic = new Product();

            topic.Id = new Guid(data["Id"].ToString());
            topic.ProductClassId = new Guid(data["ProductClassId"].ToString());
            topic.Name = data["Name"].ToString();
            topic.ShotContent = data["ShotContent"].ToString();
            topic.Image = data["Image"].ToString();
            topic.isAutoShotContent = (bool)data["isAutoShotContent"];
            topic.CreateDate = (DateTime)data["CreateDate"];
            topic.Slug = data["Slug"].ToString();
            topic.Views = (int)data["Views"];
            topic.IsLocked = (bool)data["IsLocked"];
            if (!data["Category_Id"].ToString().IsNullEmpty()) topic.Category_Id = new Guid(data["Category_Id"].ToString());
            if (!data["ProductPost_Id"].ToString().IsNullEmpty()) topic.ProductPost_Id = new Guid(data["ProductPost_Id"].ToString());
            topic.MembershipUser_Id = new Guid(data["MembershipUser_Id"].ToString());
            
            return topic;
        }

        private ProductAttribute DataRowToProductAttribute(DataRow data)
        {
            if (data == null) return null;

            var contact = new ProductAttribute();

            contact.Id = new Guid(data["Id"].ToString());
            contact.LangName = data["LangName"].ToString();
            contact.ValueType = (int)data["ValueType"];
            contact.IsNull = (bool)data["IsNull"];
            contact.IsLock = (bool)data["IsLock"];

            return contact;
        }

        private ProductClassAttribute DataRowToProductClassAttribute(DataRow data)
        {
            if (data == null) return null;

            var contact = new ProductClassAttribute();

            contact.Id = new Guid(data["Id"].ToString());
            contact.ProductAttributeId = new Guid(data["ProductAttributeId"].ToString());
            contact.ProductClassId = new Guid(data["ProductClassId"].ToString());
            contact.IsShow = (bool)data["IsShow"];

            return contact;
        }
        private ProductAttributeValue DataRowToProductAttributeValue(DataRow data)
        {
            if (data == null) return null;

            var contact = new ProductAttributeValue();

            contact.Id = new Guid(data["Id"].ToString());
            contact.ProductId = new Guid(data["ProductId"].ToString());
            contact.ProductAttributeId = new Guid(data["ProductAttributeId"].ToString());
            contact.Value = data["Value"].ToString();

            return contact;
        }

        private ProductClass DataRowToProductClass(DataRow data)
        {
            if (data == null) return null;

            var contact = new ProductClass();

            contact.Id = new Guid(data["Id"].ToString());
            contact.Name = data["Name"].ToString();
            contact.Description = data["Description"].ToString();
            contact.IsLocked = (bool)data["IsLocked"];
            contact.DateCreated = (DateTime)data["DateCreated"];
            contact.Slug = data["Slug"].ToString();
            contact.Colour = data["Colour"].ToString();
            contact.Image = data["Image"].ToString();

            return contact;
        }
        #endregion


        #region Slug
        private bool CheckSlug(string slug, DataTable data)
        {
            foreach (DataRow it in data.Rows)
            {
                if (slug == it["Slug"].ToString()) return false;
            }

            return true;
        }

        private bool TestSlug(string slug, string newslug)
        {
            if (slug == null) slug = "";
            return Regex.IsMatch(slug, string.Concat("^", newslug, "(-[\\d]+)?$"));
        }

        private void CreateSlug(Product product)
        {
            var slug = ServiceHelpers.CreateUrl(product.Name);
            if (!TestSlug(product.Slug, slug))
            {
                var tmpSlug = slug;

                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT Slug FROM [dbo].[Product] WHERE [Slug] LIKE @Slug AND [Id] != @Id ";

                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = product.Id;
                Cmd.Parameters.Add("Slug", SqlDbType.NVarChar).Value = string.Concat(slug, "%");

                DataTable data = Cmd.findAll();
                Cmd.Close();

                int i = 0;
                while (!CheckSlug(tmpSlug, data))
                {
                    i++;
                    tmpSlug = string.Concat(slug, "-", i);
                }

                product.Slug = tmpSlug;
            }
        }
        #endregion
        
        #region Product

        public void Add(Product topic)
        {
            topic.CreateDate = DateTime.UtcNow;
            CreateSlug(topic);

            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Product] WHERE [Id] = @Id)";
            Cmd.CommandText += " BEGIN INSERT INTO [Product]([Id],[ProductClassId],[Name],[ShotContent],[Image],[isAutoShotContent],[CreateDate],[Slug],[Views],[IsLocked],[Category_Id],[ProductPost_Id],[MembershipUser_Id])";
            Cmd.CommandText += " VALUES(@Id,@ProductClassId,@Name,@ShotContent,@Image,@isAutoShotContent,@CreateDate,@Slug,@Views,@IsLocked,@Category_Id,@ProductPost_Id,@MembershipUser_Id) END ";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = topic.Id;
            Cmd.Parameters.Add("ProductClassId", SqlDbType.UniqueIdentifier).Value = topic.ProductClassId;
            Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = topic.Name;
            Cmd.AddParameters("ShotContent", topic.ShotContent);
            Cmd.AddParameters("Image", topic.Image);
            Cmd.Parameters.Add("isAutoShotContent", SqlDbType.Bit).Value = topic.isAutoShotContent;

            Cmd.Parameters.Add("CreateDate", SqlDbType.DateTime).Value = topic.CreateDate;
            //Cmd.Parameters.Add("SolvedReminderSent", SqlDbType.Bit).Value = topic.SolvedReminderSent;
            Cmd.AddParameters("Slug", topic.Slug);
            Cmd.AddParameters("Views", topic.Views);
            Cmd.Parameters.Add("IsLocked", SqlDbType.Bit).Value = topic.IsLocked;
            //Cmd.AddParameters("Pending", topic.Pending);
            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = topic.Category_Id;
            Cmd.AddParameters("ProductPost_Id", topic.ProductPost_Id);
            Cmd.Parameters.Add("MembershipUser_Id", SqlDbType.UniqueIdentifier).Value = topic.MembershipUser_Id;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Add Product false");
        }

        public void Update(Product topic)
        {
            CreateSlug(topic);

            var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [dbo].[Product] SET [ProductClassId] = @ProductClassId, [Name] = @Name, [ShotContent] = @ShotContent,[Image] = @Image,[isAutoShotContent] = @isAutoShotContent, [CreateDate] = @CreateDate,"
                            + " [Slug] = @Slug, [Views] = @Views, [IsLocked] = @IsLocked, [Category_Id] = @Category_Id, [ProductPost_Id] = @ProductPost_Id, [MembershipUser_Id] = @MembershipUser_Id"
                            + " WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = topic.Id;
            Cmd.Parameters.Add("ProductClassId", SqlDbType.UniqueIdentifier).Value = topic.ProductClassId;
            Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = topic.Name;
            Cmd.AddParameters("ShotContent", topic.ShotContent);
            Cmd.AddParameters("Image", topic.Image);
            Cmd.Parameters.Add("isAutoShotContent", SqlDbType.Bit).Value = topic.isAutoShotContent;

            Cmd.Parameters.Add("CreateDate", SqlDbType.DateTime).Value = topic.CreateDate;
            //Cmd.Parameters.Add("SolvedReminderSent", SqlDbType.Bit).Value = topic.SolvedReminderSent;
            Cmd.AddParameters("Slug", topic.Slug);
            Cmd.AddParameters("Views", topic.Views);
            Cmd.Parameters.Add("IsLocked", SqlDbType.Bit).Value = topic.IsLocked;
            //Cmd.AddParameters("Pending", topic.Pending);
            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = topic.Category_Id;
            Cmd.AddParameters("ProductPost_Id", topic.ProductPost_Id);
            Cmd.Parameters.Add("MembershipUser_Id", SqlDbType.UniqueIdentifier).Value = topic.MembershipUser_Id;

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update Product false");
        }


        public void Del(Product product)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Product] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = product.Id;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Product.StartsWith);
            Cmd.Close();

        }

        public Product Get(Guid Id)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "SELECT * FROM [Product] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

            DataRow data = Cmd.findFirst();
            if (data == null) return null;

            return DataRowToProduct(data);
        }

        public Product GetBySlug(string Slug)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetBySlug-", Slug);
            var topic = _cacheService.Get<Product>(cachekey);
            if (topic == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM [dbo].[Product] WHERE [Slug] = @Slug";

                Cmd.Parameters.Add("Slug", SqlDbType.NVarChar).Value = Slug;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                topic = DataRowToProduct(data);

                _cacheService.Set(cachekey, topic, CacheTimes.OneDay);
            }
            return topic;
        }

        
        public List<Product> GetList(Guid Id, int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP "+ limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [ProductClassId] = @ProductClassId) AS MyDerivedTable WHERE RowNum > @Offset";

            Cmd.Parameters.Add("ProductClassId", SqlDbType.UniqueIdentifier).Value = Id;
            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            var rt = new List<Product>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToProduct(it));
            }

            return rt;
        }
        public int GetCount(Category cat)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForCatergory-", cat.Id);
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE Category_Id = @Category_Id";

                Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = cat.Id;

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Product> GetList(Category cat, int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [Category_Id] = @Category_Id) AS MyDerivedTable WHERE RowNum > @Offset";

            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            var rt = new List<Product>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToProduct(it));
            }

            return rt;
        }

        public List<Product> GetListForCategory(Guid Id, int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) FROM  [Product] WHERE Category_Id = @Category_Id) AS MyDerivedTable WHERE RowNum > @Offset";

            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = Id;
            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            var rt = new List<Product>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToProduct(it));
            }

            return rt;
        }

        public List<Product> GetListForClass(Guid Id, int limit = 10, int page = 1)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetListForClass-", Id);
            var list = _cacheService.Get<List<Product>>(cachekey);
            if (list == null)
            {
                var Cmd = _context.CreateCommand();

                if (page == 0) page = 1;

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE ProductClassId = @ProductClassId) AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.Parameters.Add("ProductClassId", SqlDbType.UniqueIdentifier).Value = Id;
                //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                list = new List<Product>();
                foreach (DataRow it in data.Rows)
                {
                    list.Add(DataRowToProduct(it));
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public int GetCount(ProductClass productClass)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForClass-", productClass.Id);
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE ProductClassId = @ProductClassId";

                Cmd.Parameters.Add("ProductClassId", SqlDbType.UniqueIdentifier).Value = productClass.Id;

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Product> GetList(ProductClass productClass, int limit = 10, int page = 1)
        {
            return GetListForClass(productClass.Id, limit, page);
        }

        public int GetCount()
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCount");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product]";
                
                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Product> GetList(int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product]) AS MyDerivedTable WHERE RowNum > @Offset";

            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            var rt = new List<Product>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToProduct(it));
            }

            return rt;
        }

        public int GetCount(string seach)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForCatergory-seach-", seach);
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE [Name] LIKE  N'%'+UPPER(RTRIM(LTRIM(@Seach)))+'%'";
                Cmd.Parameters.Add("Seach", SqlDbType.NVarChar).Value = seach;

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneMinute);
            }
            return (int)count;
        }
        public List<Product> GetList(string seach, int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [Name] LIKE  N'%'+UPPER(RTRIM(LTRIM(@Seach)))+'%') AS MyDerivedTable WHERE RowNum > @Offset";

            Cmd.Parameters.Add("Seach", SqlDbType.NVarChar).Value = seach;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            var rt = new List<Product>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToProduct(it));
            }

            return rt;
        }

        #endregion

        #region ProductAttributeValue
        public void Add(ProductAttributeValue cat)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [dbo].[ProductAttributeValue]([Id],[ProductId],[ProductAttributeId],[Value])"
                + " VALUES(@Id,@ProductId,@ProductAttributeId,@Value)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("ProductId", cat.ProductId);
            Cmd.AddParameters("ProductAttributeId", cat.ProductAttributeId);
            Cmd.AddParameters("Value", cat.Value);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", cat.ProductId));
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttributeValue false");
        }

        public void Set(Product product,ProductAttribute attribute,string value)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [dbo].[ProductAttributeValue] WHERE [ProductId] = @ProductId AND [ProductAttributeId] = @ProductAttributeId)";
            Cmd.CommandText += " BEGIN INSERT INTO [dbo].[ProductAttributeValue]([Id],[ProductId],[ProductAttributeId],[Value]) VALUES(@Id,@ProductId,@ProductAttributeId,@Value) END ";
            Cmd.CommandText += " ELSE BEGIN UPDATE [dbo].[ProductAttributeValue] SET [Value] = @Value WHERE [ProductId] = @ProductId AND [ProductAttributeId] = @ProductAttributeId END";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = GuidComb.GenerateComb();
            Cmd.AddParameters("ProductId", product.Id);
            Cmd.AddParameters("ProductAttributeId", attribute.Id);
            Cmd.AddParameters("Value", value);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", product.Id));
            Cmd.Close();

            if (!rt) throw new Exception("Set ProductAttributeValue false");
        }

        public List<ProductAttributeValue> GetAllAttributeValue(Guid productid)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", productid, "-GetAllAttributeValue");
            var cachedSettings = _cacheService.Get<List<ProductAttributeValue>>(cachekey);
            if (cachedSettings == null)
            {

                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[ProductAttributeValue] WHERE [ProductId] = @ProductId ";
                Cmd.AddParameters("ProductId", productid);

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                cachedSettings = new List<ProductAttributeValue>();
                foreach (DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToProductAttributeValue(it));
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneHour);
            }

            return cachedSettings;
        }
        public ProductAttributeValue GetAttributeValue(Guid productid, Guid attributeid)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", productid, "-GetAttributeValue-", attributeid);

            var cat = _cacheService.Get<ProductAttributeValue>(cachekey);
            if (cat == null)
            {
                var allcat = GetAllAttributeValue(productid);
                if (allcat == null) return null;

                foreach (ProductAttributeValue it in allcat)
                {
                    if (it.ProductAttributeId == attributeid)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneMinute);
            }
            return cat;
        }
        #endregion

        #region ProductClassAttribute
        public void Add(ProductClassAttribute cat)
        {
            var Cmd = _context.CreateCommand();
           
            Cmd.CommandText = "INSERT INTO [dbo].[ProductClassAttribute]([Id],[ProductClassId],[ProductAttributeId],[IsShow])"
                + " VALUES(@Id,@ProductClassId,@ProductAttributeId,@IsShow)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("ProductClassId", cat.ProductClassId);
            Cmd.AddParameters("ProductAttributeId", cat.ProductAttributeId);
            Cmd.AddParameters("IsShow", cat.IsShow);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", cat.ProductClassId));
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttribute false");
        }

        public List<ProductClassAttribute> GetListProductClassAttributeForProductClassId(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", id ,"-GetListProductClassAttributeForProductClassId");
            var cachedSettings = _cacheService.Get<List<ProductClassAttribute>>(cachekey);
            if (cachedSettings == null)
            {

                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[ProductClassAttribute] WHERE [ProductClassId] = @ProductClassId ";

                Cmd.AddParameters("ProductClassId", id);

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                cachedSettings = new List<ProductClassAttribute>();
                foreach (DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToProductClassAttribute(it));
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }

        public ProductClassAttribute GetProductClassAttributeForProductClassId(Guid id,Guid ProductClassId)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", ProductClassId, "-GetProductClassAttributeForProductClassId-", id);

            var cat = _cacheService.Get<ProductClassAttribute>(cachekey);
            if (cat == null)
            {
                var allcat = GetListProductClassAttributeForProductClassId(ProductClassId);
                if (allcat == null) return null;

                foreach (ProductClassAttribute it in allcat)
                {
                    if (it.Id == id)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneHour);
            }
            return cat;
        }

        public void DelAllAttributeForProductClass(Guid guid)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "DELETE FROM [dbo].[ProductClassAttribute] WHERE [ProductClassId] = @ProductClassId";

            Cmd.Parameters.Add("ProductClassId", SqlDbType.UniqueIdentifier).Value = guid;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", guid));
            Cmd.Close();
        }
        #endregion

        #region ProductClass
        public void Add(ProductClass cat)
        {
            var Cmd = _context.CreateCommand();

            cat.DateCreated = DateTime.UtcNow;
            cat.Slug = "";

            Cmd.CommandText = "INSERT INTO [dbo].[ProductClass]([Id],[Name],[Description],[IsLocked],[Slug],[Colour],[Image],[DateCreated])"
                + " VALUES(@Id,@Name,@Description,@IsLocked,@Slug,@Colour,@Image,@DateCreated)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Description", cat.Description);
            Cmd.AddParameters("IsLocked", cat.IsLocked);
            Cmd.AddParameters("Slug", cat.Slug);
            Cmd.AddParameters("Colour", cat.Colour);
            Cmd.AddParameters("Image", cat.Image);
            Cmd.AddParameters("DateCreated", cat.DateCreated);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Product.ProductClass);
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttribute false");
        }

        public void Update(ProductClass cat)
        {
            cat.Slug = "";

            var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [dbo].[ProductClass] SET [Name] = @Name,[Description] = @Description,[IsLocked] = @IsLocked,[Slug] = @Slug,[Colour] = @Colour,[Image] = @Image,[DateCreated] = @DateCreated WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Description", cat.Description);
            Cmd.AddParameters("IsLocked", cat.IsLocked);
            Cmd.AddParameters("Slug", cat.Slug);
            Cmd.AddParameters("Colour", cat.Colour);
            Cmd.AddParameters("Image", cat.Image);
            Cmd.AddParameters("DateCreated", cat.DateCreated);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update ProductClass false");
        }

        

        public ProductClass GetProductClass(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClass, "GetProductClass-", Id);
            var topic = _cacheService.Get<ProductClass>(cachekey);
            if (topic == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM [dbo].[ProductClass] WHERE [Id] = @Id";

                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                topic = DataRowToProductClass(data);

                _cacheService.Set(cachekey, topic, CacheTimes.OneDay);
            }
            return topic;
        }

        public List<ProductClass> GetAllProductClass()
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClass, "GetAllProductClass");
            var cachedSettings = _cacheService.Get<List<ProductClass>>(cachekey);
            if (cachedSettings == null)
            {

                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[ProductClass] ORDER BY [Name] ";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                cachedSettings = new List<ProductClass>();
                foreach (DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToProductClass(it));
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }
        #endregion

        #region ProductAttribute
        public void Add(ProductAttribute cat)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "INSERT INTO [dbo].[ProductAttribute]([Id],[LangName],[ValueType],[IsNull],[IsLock])"
                + " VALUES(@Id,@LangName,@ValueType,@IsNull,@IsLock)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("LangName", cat.LangName);
            Cmd.AddParameters("ValueType", cat.ValueType);
            Cmd.AddParameters("IsNull", cat.IsNull);
            Cmd.AddParameters("IsLock", cat.IsLock);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Product.Attribute);
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttribute false");
        }

        public void Update(ProductAttribute cat)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[ProductAttribute] SET [LangName] = @LangName,[ValueType] = @ValueType,[IsNull] = @IsNull,[IsLock] = @IsLock WHERE Id = @Id";


            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = cat.Id;
            Cmd.AddParameters("LangName", cat.LangName);
            Cmd.AddParameters("ValueType", cat.ValueType);
            Cmd.AddParameters("IsNull", cat.IsNull);
            Cmd.AddParameters("IsLock", cat.IsLock);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Product.Attribute);
            Cmd.Close();

            if (!rt) throw new Exception("Update ProductAttribute false");
        }

        public ProductAttribute GetAttribute(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAttribute-", id);

            var cat = _cacheService.Get<ProductAttribute>(cachekey);
            if (cat == null)
            {
                var allcat = GetAllAttribute();
                if (allcat == null) return null;

                foreach (ProductAttribute it in allcat)
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

        public ProductAttribute GetAttribute(string name)
        {
            string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAttribute-Name-", name);

            var cat = _cacheService.Get<ProductAttribute>(cachekey);
            if (cat == null)
            {
                var allcat = GetAllAttribute();
                if (allcat == null) return null;

                foreach (ProductAttribute it in allcat)
                {
                    if (it.LangName == name)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }

        public List<ProductAttribute> GetAllAttribute()
        {
            string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAllAttribute");
            var cachedSettings = _cacheService.Get<List<ProductAttribute>>(cachekey);
            if (cachedSettings == null)
            {

                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[ProductAttribute] ORDER BY [LangName] ";

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                cachedSettings = new List<ProductAttribute>();
                foreach (DataRow it in data.Rows)
                {
                    cachedSettings.Add(DataRowToProductAttribute(it));
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }
        #endregion
    }
}
