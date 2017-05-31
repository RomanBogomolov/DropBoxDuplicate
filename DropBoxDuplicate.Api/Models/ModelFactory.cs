using System;
using System.Net.Http;
using System.Web.Http.Routing;
using DropBoxDuplicate.Api.Infrastructure;
using DropBoxDuplicate.Model;

namespace DropBoxDuplicate.Api.Models
{
    public class ModelFactory
    {
        private readonly UrlHelper _urlHelper;
        public readonly CustomUserManager UserManager;

        public ModelFactory(HttpRequestMessage request, CustomUserManager appUserManager)
        {
            _urlHelper = new UrlHelper(request);
            UserManager = appUserManager;
        }

        public ModelFactory()
        {
        }

        public IdentityUserReturnModel Create(IdentityUser appUser)
        {
            return new IdentityUserReturnModel
            {
                Url = _urlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = $"{appUser.FirstName} {appUser.SecondName}",
                Email = appUser.Email,
            };
        }

        public UserFilesReturnModel CreateUserFiles(Files appFile)
        {
            return new UserFilesReturnModel
            {
                FileId = appFile.Id,
                FileName = appFile.FileName,
                CreatedDate = appFile.CreatedDate,
                FileExtension = appFile.FileExtension,
                FileSize = appFile.FileSize,
                FileType = appFile.FileType,
                LastModifyDate = appFile.LastModifyDate,
            };
        }

        public FileReturnModel Create(Files appFile)
        {
            return new FileReturnModel
            {
                UserFiles = CreateUserFiles(appFile),
                OwnerFullName = $"{appFile.User.FirstName} {appFile.User.SecondName}",
                OwnerEmail = appFile.User.Email,
                OwnerId = appFile.User.Id
            };
        }

        public FileReturnModel CreateShareFiles(Files appFile)
        {
            return new FileReturnModel
            {
                UserFiles = CreateUserFiles(appFile),
                OwnerFullName = $"{appFile.User.FirstName} {appFile.User.SecondName}",
                OwnerEmail = appFile.User.Email,
                OwnerId = appFile.User.Id
            };
        }

        public ShareReturnModel CreateShareFiles(Files appFile, AccessType type)
        {
            return new ShareReturnModel
            {
                Files = CreateShareFiles(appFile),
                AccessType = type
            };
        }
    }

    public class IdentityUserReturnModel
    {
        public string Url { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class UserFilesReturnModel
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public double? FileSize { get; set; }
        public string FileExtension { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? LastModifyDate { get; set; }
    }

    public class FileReturnModel
    {
        public UserFilesReturnModel UserFiles { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerEmail { get; set; }
    }

    public class ShareReturnModel
    {
        public FileReturnModel Files { get; set; }
        public AccessType AccessType { get; set; }
    }
}