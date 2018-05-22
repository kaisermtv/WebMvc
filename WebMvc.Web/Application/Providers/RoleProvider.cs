﻿using System.Web.Security;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Web.Application;

namespace WebMvc.Web.Membership
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        public IMembershipService MembershipService
        {
            get
            {
                return ServiceFactory.Get<IMembershipService>();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            var member = MembershipService.GetUser(username);
            if (member == null) return null;
            return MembershipService.GetRolesForUser(username);
        }

        #region NOT IMPLEMENTED - not required

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}