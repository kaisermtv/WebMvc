using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using WebMvc.Domain.Constants;
using WebMvc.Domain.DomainModel.Entities;
using WebMvc.Domain.DomainModel.Enums;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;
using WebMvc.Utilities;

namespace WebMvc.Services
{
    public partial class MembershipService : IMembershipService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILocalizationService _localizationService;

        public MembershipService(IWebMvcContext context, ICacheService cacheService, ILocalizationService localizationService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
            _localizationService = localizationService;
        }

        #region Status Codes
        public string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return _localizationService.GetResourceString("Members.Errors.DuplicateUserName");

                case MembershipCreateStatus.DuplicateEmail:
                    return _localizationService.GetResourceString("Members.Errors.DuplicateEmail");

                case MembershipCreateStatus.InvalidPassword:
                    return _localizationService.GetResourceString("Members.Errors.InvalidPassword");

                case MembershipCreateStatus.InvalidEmail:
                    return _localizationService.GetResourceString("Members.Errors.InvalidEmail");

                case MembershipCreateStatus.InvalidAnswer:
                    return _localizationService.GetResourceString("Members.Errors.InvalidAnswer");

                case MembershipCreateStatus.InvalidQuestion:
                    return _localizationService.GetResourceString("Members.Errors.InvalidQuestion");

                case MembershipCreateStatus.InvalidUserName:
                    return _localizationService.GetResourceString("Members.Errors.InvalidUserName");

                case MembershipCreateStatus.ProviderError:
                    return _localizationService.GetResourceString("Members.Errors.ProviderError");

                case MembershipCreateStatus.UserRejected:
                    return _localizationService.GetResourceString("Members.Errors.UserRejected");

                default:
                    return _localizationService.GetResourceString("Members.Errors.Unknown");
            }
        }
        #endregion

        #region DataRowToMembershipUser
        private MembershipUser DataRowToMembershipUser(DataRow data)
        {
            if (data == null) return null;

            MembershipUser member = new MembershipUser();

            member.Id = new Guid(data["Id"].ToString());
            member.UserName = data["UserName"].ToString();
            member.Password = data["Password"].ToString();
            member.PasswordSalt = data["PasswordSalt"].ToString();
            member.Email = data["Email"].ToString();
            member.PasswordQuestion = data["PasswordQuestion"].ToString();
            member.PasswordAnswer = data["PasswordAnswer"].ToString();
            member.IsApproved = (bool)data["IsApproved"];
            member.FailedPasswordAttemptCount = (int)data["FailedPasswordAttemptCount"];
            member.IsLockedOut = (bool)data["IsLockedOut"];
            member.LastLockoutDate = (DateTime)data["LastLockoutDate"];
            member.LastLoginDate = (DateTime)data["LastLoginDate"];


            return member;
        }
        #endregion

        public MembershipUser SanitizeUser(MembershipUser membershipUser)
        {
            membershipUser.Avatar = StringUtils.SafePlainText(membershipUser.Avatar);
            membershipUser.Comment = StringUtils.SafePlainText(membershipUser.Comment);
            membershipUser.Email = StringUtils.SafePlainText(membershipUser.Email);
            membershipUser.Password = StringUtils.SafePlainText(membershipUser.Password);
            membershipUser.PasswordAnswer = StringUtils.SafePlainText(membershipUser.PasswordAnswer);
            membershipUser.PasswordQuestion = StringUtils.SafePlainText(membershipUser.PasswordQuestion);
            membershipUser.Signature = StringUtils.GetSafeHtml(membershipUser.Signature, true);
            membershipUser.Twitter = StringUtils.SafePlainText(membershipUser.Twitter);
            membershipUser.UserName = StringUtils.SafePlainText(membershipUser.UserName);
            membershipUser.Website = StringUtils.SafePlainText(membershipUser.Website);
            return membershipUser;
        }

        public void Add(MembershipUser User)
        {
            //string cachekey = string.Concat(CacheKeys.Member.StartsWith, "getSetting-", key);

            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM MembershipUser WHERE Id = @Id OR UserName = @UserName OR Email = @Email)";
            Cmd.CommandText += " BEGIN INSERT INTO MembershipUser(Id,UserName,Password,PasswordSalt,Email,CreateDate,LastLockoutDate,LastPasswordChangedDate,LastLoginDate,IsLockedOut,IsApproved,IsBanned,FailedPasswordAttemptCount,FailedPasswordAnswerAttempt,Slug)";
            Cmd.CommandText += " VALUES(@Id,@UserName,@Password,@PasswordSalt,@Email,@CreateDate,@LastLockoutDate,@LastPasswordChangedDate,@LastLoginDate,@IsLockedOut,@IsApproved,@IsBanned,@FailedPasswordAttemptCount,@FailedPasswordAnswerAttempt,@Slug) END ";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = User.Id;
            Cmd.Parameters.Add("UserName", SqlDbType.NVarChar).Value = User.UserName;
            Cmd.Parameters.Add("Password", SqlDbType.NVarChar).Value = User.Password;
            Cmd.Parameters.Add("PasswordSalt", SqlDbType.NVarChar).Value = User.PasswordSalt;
            Cmd.Parameters.Add("Email", SqlDbType.NVarChar).Value = User.Email;
            Cmd.Parameters.Add("CreateDate", SqlDbType.DateTime).Value = User.CreateDate;
            Cmd.Parameters.Add("LastLockoutDate", SqlDbType.DateTime).Value = User.LastLockoutDate;
            Cmd.Parameters.Add("LastPasswordChangedDate", SqlDbType.DateTime).Value = User.LastPasswordChangedDate;
            Cmd.Parameters.Add("LastLoginDate", SqlDbType.DateTime).Value = User.LastLoginDate;
            Cmd.Parameters.Add("IsLockedOut", SqlDbType.Bit).Value = User.IsLockedOut;
            Cmd.Parameters.Add("IsApproved", SqlDbType.Bit).Value = User.IsApproved;
            Cmd.Parameters.Add("IsBanned", SqlDbType.Bit).Value = User.IsBanned;
            Cmd.Parameters.Add("FailedPasswordAttemptCount", SqlDbType.Int).Value = User.FailedPasswordAttemptCount;
            Cmd.Parameters.Add("FailedPasswordAnswerAttempt", SqlDbType.Int).Value = User.FailedPasswordAnswerAttempt;
            Cmd.Parameters.Add("Slug", SqlDbType.NVarChar).Value = User.Slug;
            

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);
            Cmd.Close();

            if (ret) throw new Exception("Add MembershipUser false");
        }

        public MembershipUser Get(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.Member.StartsWith, "Get-", Id);
            var topic = _cacheService.Get<MembershipUser>(cachekey);
            if (topic == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM [MembershipUser] WHERE Id = @Id";

                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

                DataRow data = Cmd.findFirst();
                if (data == null) return null;

                topic = DataRowToMembershipUser(data);

                _cacheService.Set(cachekey, topic, CacheTimes.OneDay);
            }
            return topic;
        }

        public MembershipCreateStatus NewUser (MembershipUser newUser)
        {
            //string cachekey = string.Concat(CacheKeys.Member.StartsWith, "getSetting-", key);

            newUser = SanitizeUser(newUser);

            var status = MembershipCreateStatus.Success;

            if (string.IsNullOrEmpty(newUser.UserName))
            {
                status = MembershipCreateStatus.InvalidUserName;
            }

            // get by username
            if (GetUser(newUser.UserName) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            // Add get by email address
            if (GetUserByEmail(newUser.Email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }

            if (string.IsNullOrEmpty(newUser.Password))
            {
                status = MembershipCreateStatus.InvalidPassword;
            }

            if (status == MembershipCreateStatus.Success)
            {
                var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                var hash = StringUtils.GenerateSaltedHash(newUser.Password, salt);
                newUser.Password = hash;
                newUser.PasswordSalt = salt;

                //newUser.Roles = new List<MembershipRole> { settings.NewMemberStartingRole };


                newUser.CreateDate = newUser.LastPasswordChangedDate = DateTime.UtcNow;
                newUser.LastLockoutDate = (DateTime)SqlDateTime.MinValue;
                newUser.LastLoginDate = DateTime.UtcNow;
                newUser.IsLockedOut = false;
                newUser.Slug = newUser.UserName;

                try
                {
                    Add(newUser);



                    
                }
                catch
                {
                    status = MembershipCreateStatus.UserRejected;
                }
                
            }


            return status;
        }

        public MembershipUser GetUserByEmail(string email)
        {
            email = StringUtils.SafePlainText(email);
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetUserByEmail-", email);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [MembershipUser] WHERE Email = @Email";

                Cmd.Parameters.Add("Email", SqlDbType.NVarChar).Value = email;

                DataRow data = Cmd.findFirst();

                Cmd.Close();

                return DataRowToMembershipUser(data);
            });

        }

        public MembershipUser GetUser(string username)
        {
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetUser-", username);
            
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT * FROM [MembershipUser] WHERE UserName = @UserName";

                Cmd.Parameters.Add("UserName", SqlDbType.NVarChar).Value = username;

                DataRow data = Cmd.findFirst();

                Cmd.Close();
                
                return DataRowToMembershipUser(data);
            });
        }

        public string[] GetRolesForUser(string username)
        {
            username = StringUtils.SafePlainText(username);

            var roles = new List<string>();
            var user = GetUser(username);

            var Cmd = _context.CreateCommand();
            Cmd.CommandText = " SELECT RL.[RoleName] FROM [MembershipRole] AS RL INNER JOIN [MembershipUsersInRoles] AS UR ON RL.Id = UR.RoleIdentifier WHERE UR.UserIdentifier = @UserId";

            Cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = user.Id;

            DataTable data = Cmd.findAll();

            Cmd.Close();

            string[] ar = new string[data.Rows.Count];
            for(int i = 0;i< data.Rows.Count;i++)
            {
                ar[i] = data.Rows[i]["RoleName"].ToString();
            }

            return ar;
        }

        /// <summary>
        /// Return last login status
        /// </summary>
        public LoginAttemptStatus LastLoginStatus { get; private set; } = LoginAttemptStatus.LoginSuccessful;

        /// <summary>
        /// Validate a user by password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="maxInvalidPasswordAttempts"> </param>
        /// <returns></returns>
        public MembershipUser ValidateUser(string userName, string password, int maxInvalidPasswordAttempts)
        {
            userName = StringUtils.SafePlainText(userName);
            password = StringUtils.SafePlainText(password);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUser(userName);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
            }

            if (user.IsBanned)
            {
                LastLoginStatus = LoginAttemptStatus.Banned;
            }

            if (user.IsLockedOut)
            {
                LastLoginStatus = LoginAttemptStatus.UserLockedOut;
            }

            if (!user.IsApproved)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotApproved;
            }

            var allowedPasswordAttempts = maxInvalidPasswordAttempts;
            if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordAttemptsExceeded;
            }

            if (LastLoginStatus == LoginAttemptStatus.LoginSuccessful )
            {
                var salt = user.PasswordSalt;
                var hash = StringUtils.GenerateSaltedHash(password, salt);
                var passwordMatches = hash == user.Password;

                user.FailedPasswordAttemptCount = passwordMatches ? 0 : user.FailedPasswordAttemptCount + 1;

                if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
                {
                    user.IsLockedOut = true;
                    user.LastLockoutDate = DateTime.UtcNow;
                }

                if (!passwordMatches)
                {
                    LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                } else
                {
                    user.LastLoginDate = DateTime.UtcNow;
                }


                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "UPDATE [MembershipUser] SET FailedPasswordAttemptCount = @FailedPasswordAttemptCount,IsLockedOut = @IsLockedOut,LastLockoutDate = @LastLockoutDate,LastLoginDate = @LastLoginDate  WHERE [Id] = @Id";

                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = user.Id;
                Cmd.Parameters.Add("FailedPasswordAttemptCount", SqlDbType.Int).Value = user.FailedPasswordAttemptCount;
                Cmd.Parameters.Add("IsLockedOut", SqlDbType.Bit).Value = user.IsLockedOut;
                Cmd.Parameters.Add("LastLockoutDate", SqlDbType.DateTime).Value = user.LastLockoutDate;
                Cmd.Parameters.Add("LastLoginDate", SqlDbType.DateTime).Value = user.LastLoginDate;

                Cmd.command.ExecuteNonQuery();

                Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);
                Cmd.Close();
            }

            if (LastLoginStatus != LoginAttemptStatus.LoginSuccessful) return null;
            return user;
        }


        public int GetCount()
        {
            string cachekey = string.Concat(CacheKeys.Member.StartsWith, "GetCount");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [MembershipUser]";

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();

                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }

        public List<MembershipUser> GetList(int limit = 10, int page = 1)
        {
            string cachekey = string.Concat(CacheKeys.Topic.StartsWith, "GetList-", limit, "-", page);
            var list = _cacheService.Get<List<MembershipUser>>(cachekey);
            if (list == null)
            {
                var Cmd = _context.CreateCommand();

                if (page == 0) page = 1;

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY UserName ASC)) AS RowNum FROM  [MembershipUser]) AS MyDerivedTable WHERE RowNum > @Offset";

                //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;

                DataTable data = Cmd.findAll();
                Cmd.Close();

                if (data == null) return null;

                list = new List<MembershipUser>();
                foreach (DataRow it in data.Rows)
                {
                    list.Add(DataRowToMembershipUser(it));
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }
    }
}
