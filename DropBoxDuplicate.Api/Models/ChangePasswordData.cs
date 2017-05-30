namespace DropBoxDuplicate.Api.Models
{
    /// <summary>
    /// Данные для изменения пароля пользователя
    /// </summary>
    public class ChangePasswordData
    {
        /// <summary>
        /// Старый пароль
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// Новый пароль
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// Подтверждение нового пароля
        /// </summary>
        public string ConfirmPassword { get; set; }

    }
}