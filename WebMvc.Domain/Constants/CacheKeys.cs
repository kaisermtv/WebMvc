using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMvc.Domain.Constants
{
    public static class CacheKeys
    {
        public static class Settings
        {
            public const string StartsWith = "Settings.";
            public static string Main = string.Concat(StartsWith, "mainsettings");
        }
        
        public static class Permission
        {
            public const string StartsWith = "Permission.";
        }

        public static class Role
        {
            public const string StartsWith = "Role.";
            public const string MembershipRole = StartsWith + "MembershipRole.";
        }

        public static class Member
        {
            public const string StartsWith = "Member.";
        }

        public static class Localization
        {
            public const string StartsWith = "Localization.";
        }

        public static class Category
        {
            public const string StartsWith = "Category.";
        }

        public static class Topic
        {
            public const string StartsWith = "Topic.";
        }

        public static class Post
        {
            public const string StartsWith = "Post.";
        }

        public static class Contact
        {
            public const string StartsWith = "Contact.";
        }
        
        public static class Booking
        {
            public const string StartsWith = "Contact.";
        }
        public static class TypeRoom
        {
            public const string StartsWith = "TypeRoom.";
        }

        public static class Product
        {
            public const string StartsWith = "Product.";
            public const string Attribute = "Product.Attribute.";
            public const string ProductClass = "Product.ProductClass.";
            public const string ProductClassAttribute = "Product.ProductClassAttribute.";
            public const string ProductAttributeValue = "Product.ProductAttributeValue.";

        }

        public static class ProductPost
        {
            public const string StartsWith = "Post.";
        }
    }
}
