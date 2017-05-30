using System.Threading.Tasks;
using DropBoxDuplicate.Api.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace DropBoxDuplicate.Api.Providers
{
    public class CustomProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            context.OwinContext.Request.Headers.Add("Access-Control-Allow-Origin", new[] {allowedOrigin});

            var userManager = context.OwinContext.GetUserManager<CustomUserManager>();
            var user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "Некорректное имя пользователя или пароль.");
                return;
            }
            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "Пользователь не подтвердил email.");
                return;
            }

            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);
        }
    }
}