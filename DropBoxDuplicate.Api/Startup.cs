using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using DropBoxDuplicate.Api.Infrastructure;
using DropBoxDuplicate.Api.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(DropBoxDuplicate.Api.Startup))]

namespace DropBoxDuplicate.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(httpConfig);
        }

       private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext<CustomUserManager>(CustomUserManager.Create);

            var oAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/connect/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(15),
                Provider = new CustomProvider(),
                AccessTokenFormat = new CustomJwtFormat(HttpContext.Current.Request.Url.AbsoluteUri)
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var issuer = HttpContext.Current.Request.Url.AbsoluteUri;
            var audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            var audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] {audienceId},

//                    Provider = new OAuthBearerAuthenticationProvider()
//                    {
//                        OnValidateIdentity = 
//                        SecurityStampValidator.OnValidateIdentity<CustomUserManager, IdentityUser, Guid>
//                        (TimeSpan.FromMinutes(30), (manager, user) => user.GenerateUserIdentityAsync(manager, this))
//
//                    },

                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    },

//                    TokenValidationParameters = new TokenValidationParameters
//                    {
//                        
//                    }
                });
        }
    }
}
