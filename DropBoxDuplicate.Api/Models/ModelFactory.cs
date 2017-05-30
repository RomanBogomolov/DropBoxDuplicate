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
    }

    public class IdentityUserReturnModel
    {
        public string Url { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}