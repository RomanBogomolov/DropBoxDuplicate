USE [master]
GO
/****** Object:  Database [DropBoxDuplicate]    Script Date: 01.06.2017 11:42:07 ******/
CREATE DATABASE [DropBoxDuplicate]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DropBoxDuplicate', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\DropBoxDuplicate.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DropBoxDuplicate_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\DropBoxDuplicate_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [DropBoxDuplicate] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DropBoxDuplicate].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DropBoxDuplicate] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET ARITHABORT OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DropBoxDuplicate] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DropBoxDuplicate] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DropBoxDuplicate] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DropBoxDuplicate] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DropBoxDuplicate] SET  MULTI_USER 
GO
ALTER DATABASE [DropBoxDuplicate] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DropBoxDuplicate] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DropBoxDuplicate] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DropBoxDuplicate] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DropBoxDuplicate] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DropBoxDuplicate] SET QUERY_STORE = OFF
GO
USE [DropBoxDuplicate]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [DropBoxDuplicate]
GO
/****** Object:  UserDefinedFunction [dbo].[ufBit_file_is_share_for_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: <Create Date, ,>
-- Description:	Является ли файл расшареным?
-- =============================================
CREATE FUNCTION [dbo].[ufBit_file_is_share_for_user]
(
	@fileId uniqueidentifier,
	@userId uniqueidentifier
)
RETURNS BIT
AS
BEGIN
	
	DECLARE @res uniqueidentifier = (SELECT idFile from dbo.Share WITH(nolock) WHERE idUser = @userId and idFile = @fileId)
	RETURN IIF(@res is not null, 1, 0)

END


GO
/****** Object:  Table [dbo].[Users]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[PasswordHash] [char](60) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[SecondName] [nvarchar](50) NULL,
	[RegDate] [datetimeoffset](7) NOT NULL,
	[City] [nvarchar](50) NULL,
	[BirthDate] [datetimeoffset](7) NULL,
	[SecurityStamp] [uniqueidentifier] NULL,
	[FullName]  AS (isnull([SecondName]+' ','')+isnull([FirstName]+' ','')),
 CONSTRAINT [PK_Users2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_user_by_id]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение информации о пользователе по ID
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_user_by_id]
(	
	@userId uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		Id
		, UserName
        , PasswordHash
		, Email
		, EmailConfirmed
		, FirstName
		, SecondName
		, RegDate
		, City
		, BirthDate
		, SecurityStamp

	FROM dbo.Users with(nolock)
	WHERE Id = @userId
)

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_user_by_username]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение информации о пользователе по username
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_user_by_username]
(	
	@userName nvarchar(50)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		Id
		, UserName
        , PasswordHash
		, Email
		, EmailConfirmed
		, FirstName
		, SecondName
		, RegDate
		, City
		, BirthDate
		, SecurityStamp

	FROM dbo.Users with(nolock)
	WHERE UserName = @userName
)

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_user_by_email]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение информации о пользователе по email
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_user_by_email]
(	
	@eMail nvarchar(50)
)
RETURNS TABLE
AS
RETURN 
(
	SELECT
		Id
		, UserName
        , PasswordHash
		, Email
		, EmailConfirmed
		, FirstName
		, SecondName
		, RegDate
		, City
		, BirthDate
		, SecurityStamp

	FROM dbo.Users with(nolock)
	WHERE Email = @eMail
)

GO
/****** Object:  Table [dbo].[Files]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Files](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[FileType] [nvarchar](50) NULL,
	[FileSize] [float] NULL,
	[FileExtension] [nvarchar](50) NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[LastModifyDate] [datetimeoffset](7) NULL,
	[FileContent] [varbinary](max) NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_file_content]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Скачивание файла
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_file_content]
(	
	@id uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		
		FileContent

	FROM dbo.Files with(nolock)
	
	WHERE Id = @id
)

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_file_info_by_id]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение информации о файле по ID
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_file_info_by_id]
(	
	@id uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT

		Id
		, UserId
        , FileName
		, FileType
		, FileSize
		, FileExtension
		, CreatedDate
		, LastModifyDate

	FROM dbo.Files with(nolock)
	WHERE Id = @id
)

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_file_info_by_userId]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение списка файлов пользователя
-- =============================================
Create FUNCTION [dbo].[ufSelect_file_info_by_userId]
(	
	@userId uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT

		Id
		, UserId
        , FileName
		, FileType
		, FileSize
		, FileExtension
		, CreatedDate
		, LastModifyDate

	FROM dbo.Files with(nolock)
	WHERE UserId = @userId
)

GO
/****** Object:  Table [dbo].[Share]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Share](
	[idUser] [uniqueidentifier] NOT NULL,
	[idFile] [uniqueidentifier] NOT NULL,
	[accessAtribute] [varchar](20) NULL,
 CONSTRAINT [PK_Share_1] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC,
	[idFile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_shareFiles_by_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение информации о расшаренных файлах для пользователя
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_shareFiles_by_user]
(	
	@userId uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT S.[idUser], S.[idFile], S.[accessAtribute] ,F.[CreatedDate], F.[FileExtension],
	F.[FileName], F.[FileSize], F.[FileType], F.[LastModifyDate]
	FROM dbo.Share S with(nolock)
	INNER JOIN dbo.Users U with(nolock) ON U.Id = S.idUser
	INNER JOIN dbo.Files F with(nolock) ON F.Id = S.idFile
	WHERE idUser = @userId
)

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_shareFiles_for_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получение информации о расшаренных файлах для пользователя
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_shareFiles_for_user]
(	
	@userId uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT idUser, idFile, accessAtribute
	FROM dbo.Share with(nolock)
	WHERE idUser = @userId
)

GO
/****** Object:  Table [dbo].[Comments]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[id] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](256) NOT NULL,
	[PostDate] [datetimeoffset](7) NOT NULL,
	[userId] [uniqueidentifier] NOT NULL,
	[fileId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_comment_by_id]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получить комментарий по Id
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_comment_by_id]
(	
	@id uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT

		Id
		, userId
		, fileId
		, Text
		, PostDate


	FROM dbo.Comments with(nolock)
	WHERE id = @id
)

GO
/****** Object:  UserDefinedFunction [dbo].[ufSelect_comments_by_fileId]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Получить комментарии для файла  
-- =============================================
CREATE FUNCTION [dbo].[ufSelect_comments_by_fileId]
(	
	@fileId uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT

		Id
		, userId
		, fileId
		, Text
		, PostDate		      

	FROM dbo.Comments with(nolock)
	WHERE fileId = @fileId
)

GO
/****** Object:  Index [IX_Email]    Script Date: 01.06.2017 11:42:07 ******/
CREATE NONCLUSTERED INDEX [IX_Email] ON [dbo].[Users]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserName]    Script Date: 01.06.2017 11:42:07 ******/
CREATE NONCLUSTERED INDEX [IX_UserName] ON [dbo].[Users]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Files] FOREIGN KEY([fileId])
REFERENCES [dbo].[Files] ([Id])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Files]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Users] FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Users]
GO
ALTER TABLE [dbo].[Files]  WITH CHECK ADD  CONSTRAINT [FK_File_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Files] CHECK CONSTRAINT [FK_File_Users]
GO
ALTER TABLE [dbo].[Share]  WITH CHECK ADD  CONSTRAINT [FK_Share_Files] FOREIGN KEY([idFile])
REFERENCES [dbo].[Files] ([Id])
GO
ALTER TABLE [dbo].[Share] CHECK CONSTRAINT [FK_Share_Files]
GO
ALTER TABLE [dbo].[Share]  WITH CHECK ADD  CONSTRAINT [FK_Share_Users] FOREIGN KEY([idUser])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Share] CHECK CONSTRAINT [FK_Share_Users]
GO
/****** Object:  StoredProcedure [dbo].[up_Delete_users_from_share]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date:
-- Description:	Удаление пользователей из шары
-- =============================================
CREATE PROCEDURE [dbo].[up_Delete_users_from_share] 

@jsonData varchar(max)

AS
BEGIN

	DELETE S FROM dbo.Share S
	JOIN OPENJSON(@jsonData) WITH
    (
      	UserId uniqueidentifier
		, FileId uniqueidentifier
    ) as t
    ON S.idFile = t.FileId AND S.idUser = t.UserId

END

GO
/****** Object:  StoredProcedure [dbo].[up_Insert_users_to_file]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date:
-- Description:	Расшаривание файла для пользователей 
-- =============================================
CREATE PROCEDURE [dbo].[up_Insert_users_to_file] 
(
	@jsonData nvarchar(max)
)
AS
BEGIN

	SET XACT_ABORT ON;

	INSERT INTO dbo.Share
	(
		idUser,
		idFile,
		accessAtribute
	)
	SELECT UserId, FileId, AccessAtribute
	FROM OPENJSON(@jsonData)
	WITH
	(
		UserId uniqueidentifier
		, FileId uniqueidentifier
		, AccessAtribute bit
	)
		
END

GO
/****** Object:  StoredProcedure [dbo].[upCreate_new_comment]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Создание нового комментария
-- =============================================
CREATE PROCEDURE [dbo].[upCreate_new_comment]
(
	@id uniqueidentifier,
	@userId uniqueidentifier,
	@fileId uniqueidentifier,
	@text nvarchar(256),
	@postDate datetimeoffset(7)
)
AS
BEGIN
	
	SET XACT_ABORT ON;

	INSERT INTO dbo.Comments
	(
		Id
		, userId
		, fileId
		, Text
		, PostDate
	)
	VALUES
	(
		@id
		, @userId
		, @fileId
		, @text
		, @postDate
	)

END

GO
/****** Object:  StoredProcedure [dbo].[upCreate_new_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Создание нового пользователя
-- =============================================
CREATE PROCEDURE [dbo].[upCreate_new_user] 
	@id uniqueidentifier,
	@userName nvarchar(50),
	@passwordHash char(60),
	@eMail nvarchar(50),
	@emailConfirmed bit,
    @firstName nvarchar(50),
	@secondName nvarchar(50),
	@regDate datetimeoffset(7),
	@city nvarchar(50),
	@birthDate datetimeoffset(7),
	@securityStamp uniqueidentifier
AS
BEGIN
	
	SET XACT_ABORT ON;

	INSERT INTO dbo.Users
	(
		Id
		, UserName
        , PasswordHash
		, Email
		, EmailConfirmed
		, FirstName
		, SecondName
		, RegDate
		, City
		, BirthDate
		, SecurityStamp
	)
	VALUES
	(
		@id
		, @userName
		, @passwordHash
		, @eMail
		, @emailConfirmed
		, @firstName
		, @secondName
		, @regDate
		, @city
		, @birthDate
		, @securityStamp
	)
		
	SELECT @id

END

GO
/****** Object:  StoredProcedure [dbo].[upCreate_UserFile]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Создание нового файла
-- =============================================
CREATE PROCEDURE [dbo].[upCreate_UserFile]
(
	@id uniqueidentifier,
	@owner uniqueidentifier,
	@fileName nvarchar(256),
	@fileType nvarchar(50) = null,
	@fileSize float = null,
	@fileExtension nvarchar(50) = null,
	@createdDate datetimeoffset(7),
	@lastModifyDate datetimeoffset(7) = null
)
AS
BEGIN
	
	SET XACT_ABORT ON;

	INSERT INTO dbo.Files
	(
		Id
		, UserId
        , FileName
		, FileType
		, FileSize
		, FileExtension
		, CreatedDate
		, LastModifyDate
	)
	VALUES
	(
		@id
		, @owner
		, @fileName
		, @fileType
		, @fileSize
		, @fileExtension
		, @createdDate
		, @lastModifyDate		
	)
		
END

GO
/****** Object:  StoredProcedure [dbo].[upDelete_comment]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date:
-- Description:	Удаление комментария
-- =============================================
CREATE PROCEDURE [dbo].[upDelete_comment]
(
	@id uniqueidentifier,
	@userId uniqueidentifier,
	@fileId uniqueidentifier
)
AS
BEGIN

	DELETE FROM dbo.Comments
	WHERE Id = @id AND userId = @userId AND fileId = @fileId

END

GO
/****** Object:  StoredProcedure [dbo].[upDelete_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date:
-- Description:	Удаление пользователя
-- =============================================
CREATE PROCEDURE [dbo].[upDelete_user]	
	@id uniqueidentifier
AS
BEGIN

	DELETE FROM dbo.Users
	WHERE Id = @id

END

GO
/****** Object:  StoredProcedure [dbo].[upDelete_UserFile]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date:
-- Description:	Удаление файла
-- =============================================
CREATE PROCEDURE [dbo].[upDelete_UserFile]	
	@id uniqueidentifier
AS
BEGIN

	DELETE FROM dbo.Files
	WHERE Id = @id

END

GO
/****** Object:  StoredProcedure [dbo].[upUpdate_access_to_file_for_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Обновление уровня доступа к файлу для пользователя
-- =============================================
CREATE PROCEDURE [dbo].[upUpdate_access_to_file_for_user]
(
	@fileId uniqueidentifier,
	@userId uniqueidentifier,
	@access bit
)
AS

BEGIN

UPDATE dbo.Share with(rowlock)

	SET 
		accessAtribute = @access
	WHERE 
		idFile = @fileId AND idUser = @userId
END


GO
/****** Object:  StoredProcedure [dbo].[upUpdate_user]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Обновление информации о профиле пользователя
-- =============================================
CREATE PROCEDURE [dbo].[upUpdate_user]
	
	@id uniqueidentifier,
	@userName nvarchar(50),
	@passwordHash char(60),
	@eMail nvarchar(50),
	@emailConfirmed bit,
    @firstName nvarchar(50),
	@secondName nvarchar(50),
	@regDate datetimeoffset(7),
	@city nvarchar(50),
	@birthDate datetimeoffset(7),
	@securityStamp uniqueidentifier
AS

BEGIN

UPDATE dbo.Users with(rowlock)

	SET 
		UserName = @userName,
		PasswordHash = @passwordHash,
		Email = @eMail,
		EmailConfirmed = @emailConfirmed,
		FirstName = @firstName,
		SecondName = @secondName,
		RegDate = @regDate,
		City = @city,
		BirthDate = @birthDate,
		SecurityStamp = @securityStamp
	WHERE 
		Id = @id
END


GO
/****** Object:  StoredProcedure [dbo].[upUpdate_UserFileContent]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Обновление файла пользователя
-- =============================================
CREATE PROCEDURE [dbo].[upUpdate_UserFileContent]
	
	@id uniqueidentifier,
	@fileContent varbinary(MAX),
	@lastModifyDate datetimeoffset(7)

AS

BEGIN

--DECLARE @lastModifyDate datetimeoffset(7) = SYSDATETIMEOFFSET()

	
UPDATE dbo.Files with(rowlock)
	
	SET 

		FileContent = @fileContent,
		LastModifyDate = @lastModifyDate		

	WHERE 

		Id = @id
END


GO
/****** Object:  StoredProcedure [dbo].[upUpdate_UserFileName]    Script Date: 01.06.2017 11:42:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Богомолов Р.В.
-- Create date: 
-- Description:	Обновление названия файла
-- =============================================
CREATE PROCEDURE [dbo].[upUpdate_UserFileName]
	
	@id uniqueidentifier,
	@fileName nvarchar(256),
	@lastModifyDate datetimeoffset(7)

AS

BEGIN

UPDATE dbo.Files with(rowlock)

	SET 

		FileName = @fileName,
		LastModifyDate = @lastModifyDate

	WHERE 

		Id = @id
END


GO
USE [master]
GO
ALTER DATABASE [DropBoxDuplicate] SET  READ_WRITE 
GO
