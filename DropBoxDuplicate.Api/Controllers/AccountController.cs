using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DropBoxDuplicate.Api.Models;
using DropBoxDuplicate.Api.Services;
using DropBoxDuplicate.Log;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;

namespace DropBoxDuplicate.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        /// <summary>
        /// Получить информацию о пользователе по Id.
        /// </summary>
        /// <remarks>
        /// Информация о пользователе
        ///  
        ///     GET: /api/account/user/E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Информация о пользователе.</returns>
        [SwaggerResponse(200, "Вернется объект", typeof(IdentityUserReturnModel))]
        [Authorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(Guid id)
        {
            var user = await AppUserManager.FindByIdAsync(id);
            return user != null ? Ok(TheModelFactory.Create(user)) 
                : GetErrorFromModel("id", "Не удается найти пользователя");
        }

        /// <summary>
        /// Получить информацию о пользователе по username.
        /// </summary>
        /// <remarks>
        /// Информация о пользователе
        ///  
        ///     GET: /api/account/user/{username}
        /// 
        /// </remarks>
        /// <param name="username">Ник</param>
        /// <returns>Информация о пользователе.</returns>
        [SwaggerResponse(200, "Вернется объект", typeof(IdentityUserReturnModel))]
        [Authorize]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await AppUserManager.FindByNameAsync(username);
            return user != null ? Ok(TheModelFactory.Create(user)) : GetErrorFromModel("username", "Не удается найти пользователя");
        }


        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     POST /api/account/create
        ///     {
        ///        "username":"testuser2",
        ///        "passwordHash":"123456",
        ///        "email":"test@gmail.com"
        ///     }
        /// 
        /// </remarks>
        /// <param name="user">Информация о пользователе</param>
        /// <returns>Новый пользователь.</returns>
        [SwaggerResponse(200, "Успешно добавлен", typeof(IdentityUser))]
        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(IdentityUser user)
        {
            var addUser = await AppUserManager.CreateAsync(user);

            if (!addUser.Succeeded)
            {
                Logger.ServiceLog.Warn("Ошибка регистрации пользователя");
                return GetErrorResult(addUser);
            }

            var code = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            code = HttpUtility.UrlEncode(code);

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code }));

            await AppUserManager.SendEmailAsync(user.Id, "Подтверждение аккаунта в DropBoxDuplicate",
                "<strong>Подтвердите ваш аккаунт</strong>: <a href=\"" + callbackUrl + "\">Ссылка</a>");

            var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            Logger.ServiceLog.Info($"Пользователь {user.Id} успешно зарегистрирован.");
            return Created(locationHeader, TheModelFactory.Create(user));
        }

        /// <summary>
        /// Подтверждение аккаунта пользователя.
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     GET: /api/account/ConfirmEmail?userId=&amp;code=
        /// 
        /// </remarks>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="code">Код для подтверждения</param>
        [SwaggerResponse(200, "Аккаунт подтвержден.")]
        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(Guid userId, string code = "")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                Logger.ServiceLog.Warn($"Пользователь {userId} не указал код.");
                return GetErrorFromModel("code", "Требуется код.");
            }
            var user = await AppUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                Logger.ServiceLog.Warn($"Пользователь с {userId} не зарегистрирован в БД");
                return GetErrorFromModel("userId", "Такого пользователя не существует.");
            }
            code = HttpUtility.UrlDecode(code);

            var result = await AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                Logger.ServiceLog.Info($"Пользователь {user.Id} успешно подтвердил аккаунт.");
                return Ok();
            }

            Logger.ServiceLog.Warn($"Пользователь с {userId} не подтвердил аккаунт");
            return GetErrorResult(result);
        }

        /// <summary>
        /// Изменение пароля пользователя.
        /// </summary>
        /// <remarks>
        /// Запрос
        /// 
        ///     POST /api/account/ChangePassword
        ///     {
        ///        "OldPassword":"testuser2",
        ///        "NewPassword":"123456",
        ///        "ConfirmPassword":"test@gmail.com"
        ///     }
        /// 
        /// </remarks>
        /// <param name="data">Данные для смены пароля</param>
        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        [SwaggerResponse(200, "Пароль успешно изменен")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordData data)
        {
            var result = await AppUserManager.ChangePasswordAsync(Guid.Parse(User.Identity.GetUserId()), data.OldPassword, data.NewPassword);

            if (result.Succeeded)
            {
                Logger.ServiceLog.Info("Пользователь успешно изменил пароль к аккаунту.");
                return Ok();
            }

            Logger.ServiceLog.Warn("Пользователь не сменил пароль.");
            return GetErrorResult(result);
        }


        /// <summary>
        /// Генерация токена и нового пороля, которые будут высланы на почту.
        /// </summary>
        /// <remarks>
        /// Запрос
        /// 
        ///     POST /api/account/RecoveryPassword?email=testuser@gmail.com
        /// 
        /// </remarks>
        /// <param name="email">Email пользователя</param>
        [SwaggerResponse(200, "Данные успешно высланы")]
        [AllowAnonymous]
        [HttpPost]
        [Route("RecoveryPassword")]
        public async Task<IHttpActionResult> RecoveryPassword(string email)
        {
            var user = await AppUserManager.FindByEmailAsync(email);

            if (user == null)
            {
                Logger.ServiceLog.Warn($"Пользователь с email {email} не зарегистрирован.");
                return GetErrorFromModel("email", $"Пользователь с email {email} не зарегистрирован.");
            }
            var recoveryToken = AppUserManager.GeneratePasswordResetTokenAsync(user.Id);
            var code = HttpUtility.UrlEncode(recoveryToken.Result);
            var password = RandomPasswordService.Generate(6);
            var callBackUri = new Uri(Url.Link("RecoveryPasswordRoute", new { userId = user.Id, code, newPassword = password }));

            await AppUserManager.SendEmailAsync(user.Id, "Восстановление пароля",
                "Ваш новый пароль: <strong>" + password + "</strong> <br> Для его подтверждения перейдите по ссылке <a href=\"" + callBackUri + "\">ССылка</a>");

            Logger.ServiceLog.Info($"Пользователю на email {email} успешно выслана инструкция по восстановдению пароля.");
            return Ok("Инструкция по смене пароля выслана на почту");
        }

        /// <summary>
        /// Подтверждение смены пароля
        /// </summary>
        /// <remarks>
        /// Запрос
        /// 
        ///     GET /api/account/ConfirmPasswordRecovery
        /// 
        /// </remarks>
        /// <param name="userId">Id пользователя</param>
        /// <param name="newPassword">Рандомно сгенерированный пароль</param>
        /// <param name="code">Token для изменения пароля</param>
        [SwaggerResponse(200, "Пароль успешно изменен.")]
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmPasswordRecovery", Name = "RecoveryPasswordRoute")]
        public IHttpActionResult ConfirmPasswordRecovery(Guid userId, string newPassword = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                Logger.ServiceLog.Warn($"Пользователь {userId} не указал код.");
                return GetErrorFromModel("code", "Требуется код.");
            }
            var user = AppUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                Logger.ServiceLog.Warn($"Пользователь с id {userId} не зарегистрирован.");
                GetErrorFromModel("userId", $"Пользователь с id {userId} не зарегистрирован.");
            }
            code = HttpUtility.UrlDecode(code);
            var result = AppUserManager.ResetPassword(userId, code, newPassword);

            if (result.Succeeded)
            {
                Logger.ServiceLog.Info($"Пользователю {userId} успешно изменил пароль.");
                return Ok();
            }

            Logger.ServiceLog.Warn($"Пользователь с id {userId} не смог изменить пароль.");
            return GetErrorResult(result);
        }
    }
}
