using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using DropBoxDuplicate.Api.Infrastructure;
using DropBoxDuplicate.Api.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace DropBoxDuplicate.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        private ModelFactory _modelFactory;
        private readonly CustomUserManager _userManager = null;
        protected CustomUserManager AppUserManager => _userManager ??
                                                      HttpContext.Current.GetOwinContext().Get<CustomUserManager>();
        protected ModelFactory TheModelFactory => _modelFactory ??
                                                  (_modelFactory = new ModelFactory(Request, AppUserManager));

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }
            if (result.Succeeded)
            {
                return null;
            }
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error);
                }
            }
            if (ModelState.IsValid)
            {
                /*
                 * Ошибки ModelState недоступны для отправки, поэтому просто возвращаем пустой BadRequest.
                 */
                return BadRequest();
            }

            var modelState = new ApiResourceValidationErrorWrapper(ModelState);
            var json = JsonConvert.SerializeObject(modelState);

            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }

        protected IHttpActionResult GetErrorFromModel(string model, string text)
        {
            ModelState.AddModelError(model, text);
            var modelState = new ApiResourceValidationErrorWrapper(ModelState);
            var json = JsonConvert.SerializeObject(modelState);
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }
}
