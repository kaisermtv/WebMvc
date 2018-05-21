﻿using System;
using System.Collections.Generic;
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
    public partial class TopicService : ITopicService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public TopicService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private Topic DataRowToTopic(DataRow data)
        {
            if (data == null) return null;

            Topic topic = new Topic();

            topic.Id = new Guid(data["Id"].ToString());
            topic.Name = data["Name"].ToString();
            topic.ShotContent = data["ShotContent"].ToString();
            topic.Image = data["Image"].ToString();
            topic.isAutoShotContent = (bool)data["isAutoShotContent"];
            topic.CreateDate = (DateTime)data["CreateDate"];
            topic.Solved = (bool)data["Solved"];
            if (!data["SolvedReminderSent"].ToString().IsNullEmpty())  topic.SolvedReminderSent = (bool)data["SolvedReminderSent"];
            topic.Slug = data["Slug"].ToString();
            topic.Views = (int)data["Views"];
            topic.IsSticky = (bool)data["IsSticky"];
            topic.IsLocked = (bool)data["IsLocked"]; 
            if(!data["Category_Id"].ToString().IsNullEmpty()) topic.Category_Id = new Guid(data["Category_Id"].ToString());
            if (!data["Post_Id"].ToString().IsNullEmpty()) topic.Post_Id = new Guid(data["Post_Id"].ToString());
            if (!data["Poll_Id"].ToString().IsNullEmpty()) topic.Poll_Id = new Guid(data["Poll_Id"].ToString());
            topic.MembershipUser_Id = new Guid(data["MembershipUser_Id"].ToString());
            //topic.iType = (int)data["iType"];


            return topic;
        }
        #endregion

        public void Add(Topic topic)
        {
            topic.CreateDate = DateTime.UtcNow;
            topic.Slug = "";

            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText += " BEGIN INSERT INTO [Topic]([Id],[Name],[ShotContent],[Image],[isAutoShotContent],[CreateDate],[Solved],[SolvedReminderSent],[Slug],[Views],[IsSticky],[IsLocked],[Category_Id],[Post_Id],[Poll_Id],[MembershipUser_Id],[iType])";
            Cmd.CommandText += " VALUES(@Id,@Name,@ShotContent,@Image,@isAutoShotContent,@CreateDate,@Solved,@SolvedReminderSent,@Slug,@Views,@IsSticky,@IsLocked,@Category_Id,@Post_Id,@Poll_Id,@MembershipUser_Id,@iType) END ";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = topic.Id;
            Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = topic.Name;
            Cmd.AddParameters("ShotContent", topic.ShotContent);
            Cmd.AddParameters("Image", topic.Image);
            Cmd.Parameters.Add("isAutoShotContent", SqlDbType.Bit).Value = topic.isAutoShotContent;
            
            Cmd.Parameters.Add("CreateDate", SqlDbType.DateTime).Value = topic.CreateDate;
            Cmd.Parameters.Add("Solved", SqlDbType.Bit).Value = topic.Solved;
            Cmd.AddParameters("SolvedReminderSent", topic.SolvedReminderSent);
            //Cmd.Parameters.Add("SolvedReminderSent", SqlDbType.Bit).Value = topic.SolvedReminderSent;
            Cmd.AddParameters("Slug",topic.Slug);
            Cmd.AddParameters("Views", topic.Views);
            Cmd.Parameters.Add("IsSticky", SqlDbType.Bit).Value = topic.IsSticky;
            Cmd.Parameters.Add("IsLocked", SqlDbType.Bit).Value = topic.IsLocked;
            //Cmd.AddParameters("Pending", topic.Pending);
            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = topic.Category_Id;
            Cmd.AddParameters("Post_Id", topic.Post_Id);
            Cmd.AddParameters("Poll_Id", topic.Poll_Id);
            Cmd.Parameters.Add("MembershipUser_Id", SqlDbType.UniqueIdentifier).Value = topic.MembershipUser_Id;
            Cmd.AddParameters("iType", 0);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Add Topic false");
        }

        public void Update(Topic topic)
        {
            topic.CreateDate = DateTime.UtcNow;
            topic.Slug = "";
            
            var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [Topic] SET [Name] = @Name, [ShotContent] = @ShotContent,[Image] = @Image,[isAutoShotContent] = @isAutoShotContent, [CreateDate] = @CreateDate, [Solved] = @Solved, [SolvedReminderSent] = @SolvedReminderSent,"
                            + " [Slug] = @Slug, [Views] = @Views, [IsSticky] = @IsSticky, [IsLocked] = @IsLocked, [Category_Id] = @Category_Id, [Post_Id] = @Post_Id, [Poll_Id] = @Poll_Id, [MembershipUser_Id] = @MembershipUser_Id, [iType] = @iType"
                            + " WHERE [Id] = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = topic.Id;
            Cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = topic.Name;
            Cmd.AddParameters("ShotContent", topic.ShotContent);
            Cmd.AddParameters("Image", topic.Image);
            Cmd.Parameters.Add("isAutoShotContent", SqlDbType.Bit).Value = topic.isAutoShotContent;

            Cmd.Parameters.Add("CreateDate", SqlDbType.DateTime).Value = topic.CreateDate;
            Cmd.Parameters.Add("Solved", SqlDbType.Bit).Value = topic.Solved;
            Cmd.AddParameters("SolvedReminderSent", topic.SolvedReminderSent);
            //Cmd.Parameters.Add("SolvedReminderSent", SqlDbType.Bit).Value = topic.SolvedReminderSent;
            Cmd.AddParameters("Slug", topic.Slug);
            Cmd.AddParameters("Views", topic.Views);
            Cmd.Parameters.Add("IsSticky", SqlDbType.Bit).Value = topic.IsSticky;
            Cmd.Parameters.Add("IsLocked", SqlDbType.Bit).Value = topic.IsLocked;
            //Cmd.AddParameters("Pending", topic.Pending);
            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = topic.Category_Id;
            Cmd.AddParameters("Post_Id", topic.Post_Id);
            Cmd.AddParameters("Poll_Id", topic.Poll_Id);
            Cmd.Parameters.Add("MembershipUser_Id", SqlDbType.UniqueIdentifier).Value = topic.MembershipUser_Id;
            Cmd.AddParameters("iType", 0);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Add Topic false");
        }

        public Topic Get(Guid Id)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "SELECT * FROM [Topic] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

            DataRow data = Cmd.findFirst();
            if (data == null) return null;

            return DataRowToTopic(data);
        }

        public List<Topic> GetList(Guid Id,int limit = 10,int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP @limit * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) FROM  [Topic] WHERE Category_Id = @Category_Id) AS MyDerivedTable WHERE RowNum > @Offset";

            Cmd.Parameters.Add("Category_Id", SqlDbType.UniqueIdentifier).Value = Id;
            Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page-1)* limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            List<Topic> rt = new List<Topic>();
            foreach(DataRow it in data.Rows)
            {
                rt.Add(DataRowToTopic(it));
            }

            return rt;
        }

        public List<Topic> GetList(int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP "+ limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Topic]) AS MyDerivedTable WHERE RowNum > @Offset";
            
            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

            DataTable data = Cmd.findAll();
            Cmd.Close();

            if (data == null) return null;

            List<Topic> rt = new List<Topic>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToTopic(it));
            }

            return rt;
        }
    }
}