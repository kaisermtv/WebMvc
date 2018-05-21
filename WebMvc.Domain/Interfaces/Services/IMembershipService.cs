using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.DomainModel.Entities;

namespace WebMvc.Domain.Interfaces.Services
{
    public enum LoginAttemptStatus
    {
        LoginSuccessful,
        UserNotFound,
        PasswordIncorrect,
        PasswordAttemptsExceeded,
        UserLockedOut,
        UserNotApproved,
        Banned
    }

    public partial interface IMembershipService
    {
        string ErrorCodeToString(MembershipCreateStatus createStatus);
        MembershipUser GetUser(string username);
        MembershipCreateStatus NewUser(MembershipUser newUser);
        string[] GetRolesForUser(string username);

        LoginAttemptStatus LastLoginStatus { get; }
        MembershipUser ValidateUser(string userName, string password, int maxInvalidPasswordAttempts);
    }
}
