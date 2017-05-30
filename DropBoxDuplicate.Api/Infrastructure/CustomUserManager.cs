using System;
using System.Configuration;
using DropBoxDuplicate.Api.Services;
using DropBoxDuplicate.DataAccess.Sql;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;

namespace DropBoxDuplicate.Api.Infrastructure
{
    public class CustomUserManager : UserManager<IdentityUser, Guid>
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DBD"].ConnectionString;

        public CustomUserManager(IUserStore<IdentityUser, Guid> store)
            : base(store)
        {
        }

        public static CustomUserManager Create(IdentityFactoryOptions<CustomUserManager> options, IOwinContext context)
        {
            var manager = new CustomUserManager(new IdentityUserRepository(connectionString))
            {
                EmailService = new EmailService(),
                PasswordHasher = new PasswordService()
            };

            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<IdentityUser, Guid>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromHours(1)
                    };
            }

            manager.UserValidator = new UserValidator<IdentityUser, Guid>(manager)
            {
                RequireUniqueEmail = true,                    // валидный, уникальный и не пустой email
            };

            return manager;
        }
    }
}