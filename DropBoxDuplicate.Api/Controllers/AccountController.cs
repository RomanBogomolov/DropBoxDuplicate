using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DropBoxDuplicate.Api.Models;
using DropBoxDuplicate.Api.Services;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;

namespace DropBoxDuplicate.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        //GET: /api/account/user/{id}
        [Authorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(Guid id)
        {
            var user = await AppUserManager.FindByIdAsync(id);
            return user != null ? Ok(TheModelFactory.Create(user)) : GetErrorFromModel("id", "Не удается найти пользователя");
        }

        //GET: /api/account/user/{username}
        [Authorize]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await AppUserManager.FindByNameAsync(username);
            return user != null ? Ok(TheModelFactory.Create(user)) : GetErrorFromModel("username", "Не удается найти пользователя");
        }
        
        //POST: /api/account/create
        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(IdentityUser user)
        {
            var addUser = await AppUserManager.CreateAsync(user);

            if (!addUser.Succeeded)
            {
                return GetErrorResult(addUser);
            }

            var code = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            code = HttpUtility.UrlEncode(code);

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code }));
            /**/
            await AppUserManager.SendEmailAsync(user.Id, "Подтверждение аккаунта в DropBoxDuplicate",
                "<strong>Подтвердите ваш аккаунт</strong>: <a href=\"" + callbackUrl + "\">Ссылка</a>");

            var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        //GET: /api/account/ConfirmEmail?userId=&code=
        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(Guid userId, string code = "")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return GetErrorFromModel("code", "Требуется код.");
            }
            var user = await AppUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return GetErrorFromModel("userId", "Такого пользователя не существует.");
            }
            code = HttpUtility.UrlDecode(code);

            IdentityResult result = await AppUserManager.ConfirmEmailAsync(userId, code);

            return result.Succeeded ? Ok("Ваш аккаунт подтвержден.") : GetErrorResult(result);
        }

        //POST: /api/account/ChangePassword

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordData data)
        {
            var result = await AppUserManager.ChangePasswordAsync(Guid.Parse(User.Identity.GetUserId()), data.OldPassword, data.NewPassword);

            return !result.Succeeded ? GetErrorResult(result) : Ok("Пароль успешно изменен.");
        }

        //POST: /api/account/RecoveryPassword?email=xxxx
        /// <summary>
        /// Генерация токена и нового пороля, которые будут высланы на почту
        /// </summary>
        /// <param name="email">eMail пользователя</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("RecoveryPassword")]
        public async Task<IHttpActionResult> RecoveryPassword(string email)
        {
            var user = await AppUserManager.FindByEmailAsync(email);

            if (user == null)
            {
                return GetErrorFromModel("email", $"Пользователь с email {email} не зарегистрирован.");
            }
            var recoveryToken = AppUserManager.GeneratePasswordResetTokenAsync(user.Id);
            var code = HttpUtility.UrlEncode(recoveryToken.Result);
            var password = RandomPasswordService.Generate(6);
            var callBackUri = new Uri(Url.Link("RecoveryPasswordRoute", new { userId = user.Id, code, newPassword = password }));

            await AppUserManager.SendEmailAsync(user.Id, "Восстановление пароля",
                "Ваш новый пароль: <strong>" + password + "</strong> <br> Для его подтверждения перейдите по ссылке <a href=\"" + callBackUri + "\">ССылка</a>");

            return Ok("Инструкция по смене пароля выслана на почту");
        }

        //GET: /api/account/ConfirmPasswordRecovery
        /// <summary>
        /// Подтверждение смены пароля
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="newPassword">Рандомно сгенерированный пароль</param>
        /// <param name="code">Token для изменения пароля</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmPasswordRecovery", Name = "RecoveryPasswordRoute")]
        public IHttpActionResult ConfirmPasswordRecovery(Guid userId, string newPassword = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return GetErrorFromModel("code", "Требуется код.");
            }
            var user = AppUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                GetErrorFromModel("userId", $"Пользователь с id {userId} не зарегистрирован.");
            }
            code = HttpUtility.UrlDecode(code);
            var result = AppUserManager.ResetPassword(userId, code, newPassword);

            return result.Succeeded ? Ok("Вы успешно сменили пароль.") : GetErrorResult(result);
        }
    }
}
