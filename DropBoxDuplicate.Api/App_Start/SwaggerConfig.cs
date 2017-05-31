using System.Web.Http;
using WebActivatorEx;
using DropBoxDuplicate.Api;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DropBoxDuplicate.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "DropBoxDuplicate.Api");

                        c.IncludeXmlComments(string.Format(string.Format(@"{0}\bin\DropBoxDuplicate.Api.xml",
                            System.AppDomain.CurrentDomain.BaseDirectory)));

                        c.DescribeAllEnumsAsStrings();
                    })
                .EnableSwaggerUi(c =>
                    {
                      
                    });
        }
    }
}
