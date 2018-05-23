CREATE TABLE [dbo].[tblSetting](
	[STKEY] [nvarchar](30) NOT NULL PRIMARY KEY,
	[VALUE] [ntext] NULL,
) 
INSERT INTO [dbo].[tblSetting]([STKEY],[VALUE]) VALUES(N'Theme',N'Gland')


CREATE TABLE [dbo].[MembershipUser](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[UserName] [nvarchar](150) NOT NULL UNIQUE,
	[Password] [nvarchar](128) NOT NULL ,
	[PasswordSalt] [nvarchar](128) NULL,
	[Email] [nvarchar](256) NULL,
	[PasswordQuestion] [nvarchar](256) NULL,
	[PasswordAnswer] [nvarchar](256) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[IsBanned] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[LastPasswordChangedDate] [datetime] NOT NULL,
	[LastLockoutDate] [datetime] NOT NULL,
	[LastActivityDate] [datetime] NULL,
	[FailedPasswordAttemptCount] [int] NOT NULL,
	[FailedPasswordAnswerAttempt] [int] NOT NULL,
	[PasswordResetToken] [nvarchar](150) NULL,
	[PasswordResetTokenCreatedAt] [datetime] NULL,
	[Comment] [nvarchar](max) NULL,
	[Slug] [nvarchar](150) NOT NULL,
	[Signature] [nvarchar](1000) NULL,
	[Age] [int] NULL,
	[Location] [nvarchar](100) NULL,
	[Website] [nvarchar](100) NULL,
	[Twitter] [nvarchar](60) NULL,
	[Facebook] [nvarchar](60) NULL,
	[Avatar] [nvarchar](500) NULL,
	[FacebookAccessToken] [nvarchar](300) NULL,
	[FacebookId] [bigint] NULL,
	[TwitterAccessToken] [nvarchar](300) NULL,
	[TwitterId] [nvarchar](150) NULL,
	[GoogleAccessToken] [nvarchar](300) NULL,
	[GoogleId] [nvarchar](150) NULL,
	[MicrosoftAccessToken] [nvarchar](450) NULL,
	[MicrosoftId] [nvarchar](max) NULL,
	[IsExternalAccount] [bit] NULL,
	[TwitterShowFeed] [bit] NULL,
	[LoginIdExpires] [datetime] NULL,
	[MiscAccessToken] [nvarchar](250) NULL,
	[DisableEmailNotifications] [bit] NULL,
	[DisablePosting] [bit] NULL,
	[DisablePrivateMessages] [bit] NULL,
	[DisableFileUploads] [bit] NULL,
	[HasAgreedToTermsAndConditions] [bit] NULL,
	[Latitude] [nvarchar](40) NULL,
	[Longitude] [nvarchar](40) NULL,
);

INSERT [dbo].[MembershipUser] ([Id], [UserName], [Password], [PasswordSalt], [Email], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [IsBanned], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [LastActivityDate], [FailedPasswordAttemptCount], [FailedPasswordAnswerAttempt], [PasswordResetToken], [PasswordResetTokenCreatedAt], [Comment], [Slug], [Signature], [Age], [Location], [Website], [Twitter], [Facebook], [Avatar], [FacebookAccessToken], [FacebookId], [TwitterAccessToken], [TwitterId], [GoogleAccessToken], [GoogleId], [MicrosoftAccessToken], [MicrosoftId], [IsExternalAccount], [TwitterShowFeed], [LoginIdExpires], [MiscAccessToken], [DisableEmailNotifications], [DisablePosting], [DisablePrivateMessages], [DisableFileUploads], [HasAgreedToTermsAndConditions], [Latitude], [Longitude]) 
	VALUES (N'687abc70-f93d-49d4-985c-a8af00dccdad', N'admin', N'wIJuGJDB9b/EvK1firQp/gTtjPBEpUy7G+RyJmXUfbY=', N'IBAqGIIeeKFo8TFbVf8M+V9lgSGKH2Et', N'admin@yahoo.com', NULL, NULL, 1, 0, 0, CAST(N'2018-03-26 13:23:55.370' AS DateTime), CAST(N'2018-03-27 09:14:56.693' AS DateTime), CAST(N'2018-03-26 13:23:55.370' AS DateTime), CAST(N'1753-01-01 00:00:00.000' AS DateTime), NULL, 0, 0, NULL, NULL, NULL, N'kaisermtv', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)

CREATE TABLE [dbo].[MembershipRole](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[RoleName] [nvarchar](256) NOT NULL,
);

INSERT INTO [dbo].[MembershipRole]([Id],[RoleName])VALUES('1D42BBBA-834F-457F-AF32-A827003B2D1F','Admin')
INSERT INTO [dbo].[MembershipRole]([Id],[RoleName])VALUES('FD95507D-4002-4C5B-8A64-A827003B2D22','Standard Members')
INSERT INTO [dbo].[MembershipRole]([Id],[RoleName])VALUES('3D2B6D1C-74F4-4ABF-9378-A827003B2D24','Guest')

CREATE TABLE [dbo].[MembershipUsersInRoles](
	[UserIdentifier] [uniqueidentifier] NOT NULL,
	[RoleIdentifier] [uniqueidentifier] NOT NULL,
) 
INSERT [dbo].[MembershipUsersInRoles] ([UserIdentifier], [RoleIdentifier]) 
	VALUES (N'687abc70-f93d-49d4-985c-a8af00dccdad', N'1d42bbba-834f-457f-af32-a827003b2d1f')



CREATE TABLE [dbo].[Language](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[LanguageCulture] [nvarchar](20) NOT NULL,
	[FlagImageFileName] [nvarchar](50) NULL,
	[RightToLeft] [bit] NOT NULL,
)

INSERT INTO [dbo].[Language]([Id],[Name],[LanguageCulture],[FlagImageFileName],[RightToLeft])
     VALUES('9B4F55B2-589D-4200-B862-A8270040EA71','Vietnamese (Vietnam)','vi-VN',null,0)

CREATE TABLE [dbo].[LocaleResourceKey](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](200) NOT NULL UNIQUE,
	[Notes] [nvarchar](max) NULL,
	[DateAdded] [datetime] NOT NULL,
)

CREATE TABLE [dbo].[LocaleStringResource](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[ResourceValue] [nvarchar](1000) NOT NULL,
	[LocaleResourceKey_Id] [uniqueidentifier] NOT NULL,
	[Language_Id] [uniqueidentifier] NOT NULL,
)


CREATE TABLE [dbo].[Category](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](450) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsLocked] [bit] NOT NULL,
	[ModerateTopics] [bit] NOT NULL,
	[ModeratePosts] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Slug] [nvarchar](450) NOT NULL UNIQUE,
	[PageTitle] [nvarchar](80) NULL,
	[Path] [nvarchar](2500) NULL,
	[MetaDescription] [nvarchar](200) NULL,
	[Colour] [nvarchar](50) NULL,
	[Image] [nvarchar](200) NULL,
	[Category_Id] [uniqueidentifier] NULL,
)


CREATE TABLE [dbo].[Topic](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](450) NOT NULL,
	[ShotContent] [nvarchar](450) NULL,
	[isAutoShotContent] [bit] NOT NULL,
	[Image] [nvarchar](200) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Solved] [bit] NOT NULL,
	[SolvedReminderSent] [bit] NULL,
	[Slug] [nvarchar](450) NOT NULL UNIQUE,
	[Views] [int] NULL,
	[IsSticky] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[Pending] [bit] NULL,
	[Category_Id] [uniqueidentifier] NOT NULL,
	[Post_Id] [uniqueidentifier] NULL,
	[Poll_Id] [uniqueidentifier] NULL,
	[MembershipUser_Id] [uniqueidentifier] NOT NULL
)

CREATE TABLE [dbo].[Post](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[PostContent] [nvarchar](max) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[VoteCount] [int] NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[IsSolution] [bit] NOT NULL,
	[IsTopicStarter] [bit] NULL,
	[FlaggedAsSpam] [bit] NULL,
	[IpAddress] [nvarchar](50) NULL,
	[Pending] [bit] NULL,
	[SearchField] [nvarchar](max) NULL,
	[InReplyTo] [uniqueidentifier] NULL,
	[Topic_Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [uniqueidentifier] NOT NULL,
)

CREATE TABLE [dbo].[ProductClass](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](450) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsLocked] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Slug] [nvarchar](450) NOT NULL,
	[Colour] [nvarchar](50) NULL,
	[Image] [nvarchar](200) NULL,
)

CREATE TABLE [dbo].[ProductAttribute](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[LangName] [nvarchar](200),
	[ValueType] INT NOT NULL,
	[IsNull] [bit] NOT NULL,
	[IsLock] [bit] NOT NULL
)


CREATE TABLE [dbo].[ProductClassAttribute](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[ProductClassId] [uniqueidentifier] NOT NULL,
	[ProductAttributeId] [uniqueidentifier] NOT NULL,
	[IsShow] [bit] NOT NULL,
)

CREATE TABLE [dbo].[Product](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[ProductClassId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[ShotContent] [nvarchar](450) NULL,
	[isAutoShotContent] [bit] NOT NULL,
	[Image] [nvarchar](200) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Slug] [nvarchar](450) NOT NULL,
	[Views] [int] NULL,
	[IsLocked] [bit] NOT NULL,
	[Pending] [bit] NULL,
	[Category_Id] [uniqueidentifier] NOT NULL,
	[ProductPost_Id] [uniqueidentifier] NULL,
	[MembershipUser_Id] [uniqueidentifier] NOT NULL,
)

CREATE TABLE [dbo].[ProductAttributeValue](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductAttributeId] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](max) NULL
)

CREATE TABLE [dbo].[ProductPost](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[PostContent] [nvarchar](max) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[VoteCount] [int] NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[IsSolution] [bit] NOT NULL,
	[IsTopicStarter] [bit] NULL,
	[FlaggedAsSpam] [bit] NULL,
	[IpAddress] [nvarchar](50) NULL,
	[Pending] [bit] NULL,
	[SearchField] [nvarchar](max) NULL,
	[InReplyTo] [uniqueidentifier] NULL,
	[Product_Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [uniqueidentifier] NOT NULL,
)


CREATE TABLE [dbo].[Permission](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](150) NOT NULL,
	[IsGlobal] [bit] NOT NULL,
)


CREATE TABLE [dbo].[Contact](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](150) NULL,
	[Email] [nvarchar](150) NULL,
	[Content] [nvarchar](max) NULL,
	[IsCheck] [bit] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
)


CREATE TABLE [dbo].[TypeRoom](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](150) NULL,
	[IsShow] [bit] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
)

CREATE TABLE [dbo].[Booking](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[NamePartner] [nvarchar](150) NULL,
	[CheckIn] [datetime] NULL,
	[CheckOut] [datetime] NULL,
	[Adukts] [int] NULL,
	[Adolescent] [int] NULL,
	[Children] [int] NULL,
	[TypeRoom_Id] [uniqueidentifier] NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](150) NULL,
	[IsCheck] [bit] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
)

