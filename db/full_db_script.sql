USE [master]
GO
/****** Object:  Database [DropBoxDuplicate]    Script Date: 02.05.2017 16:46:15 ******/
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
/****** Object:  Table [dbo].[Comments]    Script Date: 02.05.2017 16:46:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[id] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](256) NOT NULL,
	[PostDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[File]    Script Date: 02.05.2017 16:46:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[File](
	[Id] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[FileType] [nvarchar](50) NULL,
	[FileSize] [bigint] NOT NULL,
	[Data] [varbinary](max) NOT NULL,
	[CreatedDay] [datetimeoffset](7) NOT NULL,
	[FileExtension] [nvarchar](50) NULL,
	[LastModifyDay] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Share]    Script Date: 02.05.2017 16:46:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Share](
	[id] [uniqueidentifier] NOT NULL,
	[idUser] [uniqueidentifier] NOT NULL,
	[idUserFile] [uniqueidentifier] NOT NULL,
	[commentId] [uniqueidentifier] NULL,
	[accessAtribute] [varchar](20) NULL,
 CONSTRAINT [PK_Share] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserFiles]    Script Date: 02.05.2017 16:46:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserFiles](
	[idUserFile] [uniqueidentifier] NOT NULL,
	[userId] [uniqueidentifier] NOT NULL,
	[fileId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserFiles] PRIMARY KEY CLUSTERED 
(
	[idUserFile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 02.05.2017 16:46:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [varchar](255) NULL,
	[SecondName] [varchar](255) NULL,
	[UserName] [varchar](255) NOT NULL,
	[Password] [char](60) NOT NULL,
	[Email] [nchar](10) NULL,
	[RegDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_Users2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Share]  WITH CHECK ADD  CONSTRAINT [FK_Share_UserFiles] FOREIGN KEY([idUserFile])
REFERENCES [dbo].[UserFiles] ([idUserFile])
GO
ALTER TABLE [dbo].[Share] CHECK CONSTRAINT [FK_Share_UserFiles]
GO
ALTER TABLE [dbo].[Share]  WITH CHECK ADD  CONSTRAINT [FK_UserShare_Comments] FOREIGN KEY([commentId])
REFERENCES [dbo].[Comments] ([id])
GO
ALTER TABLE [dbo].[Share] CHECK CONSTRAINT [FK_UserShare_Comments]
GO
ALTER TABLE [dbo].[Share]  WITH CHECK ADD  CONSTRAINT [FK_UserShare_Users] FOREIGN KEY([idUser])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Share] CHECK CONSTRAINT [FK_UserShare_Users]
GO
ALTER TABLE [dbo].[UserFiles]  WITH CHECK ADD  CONSTRAINT [FK_UserFiles_File] FOREIGN KEY([fileId])
REFERENCES [dbo].[File] ([Id])
GO
ALTER TABLE [dbo].[UserFiles] CHECK CONSTRAINT [FK_UserFiles_File]
GO
ALTER TABLE [dbo].[UserFiles]  WITH CHECK ADD  CONSTRAINT [FK_UserFiles_Users] FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserFiles] CHECK CONSTRAINT [FK_UserFiles_Users]
GO
USE [master]
GO
ALTER DATABASE [DropBoxDuplicate] SET  READ_WRITE 
GO
