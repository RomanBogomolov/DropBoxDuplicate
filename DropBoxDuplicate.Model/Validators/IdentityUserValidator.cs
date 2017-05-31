using System.Text.RegularExpressions;
using FluentValidation;

namespace DropBoxDuplicate.Model.Validators
{
    public class IdentityUserValidator : AbstractValidator<IdentityUser>
    {
        public IdentityUserValidator()
        {
            /* Логин */
            RuleFor(user => user.UserName)
                .NotEmpty()
                .WithMessage("Логин не может быть пустым.")
                .Must(CheckUserName)
                .WithMessage("Некорректный логин.");

            /* eMail */
            RuleFor(user => user.Email)
                .Length(6, 50)
                .WithMessage("Email не может быть меньше 6 и больше 50 символов.")
                .EmailAddress()
                .WithMessage("Неверный формат eMail.");

            /* Пароль */
            RuleFor(user => user.PasswordHash)
                .NotEmpty()
                .WithMessage("Пароль не может быть пустым.")
                .Length(6, 30)
                .WithMessage("Пароль не может быть меньше 6 и больше 30 символов.");

            /* Имя пользователя и пароль */
            RuleFor(user => user.UserName)
                .NotEqual(x => x.PasswordHash)
                .WithMessage("Логин не должен совпадать с Паролем.");
            
            /* Имя */
            RuleFor(user => user.FirstName)
                .Must(CheckFieldCharacter)
                .WithMessage("Имя может состоять из символов руского, английского алфавита и пробелов")
                .Length(0, 50)
                .WithMessage("Длина поля не должна превышать 50 символов"); 
            
            /* Фамилия */
            RuleFor(user => user.SecondName)
                .Must(CheckFieldCharacter)
                .WithMessage("Фамилия может состоять из символов руского, английского алфавита и пробелов")
                .Length(0, 50)
                .WithMessage("Длина поля не должна превышать 50 символов");            
            
            /* Город */
            RuleFor(user => user.City)
                .Must(CheckFieldCharacter)
                .WithMessage("Город может состоять из символов руского, английского алфавита и пробелов")
                .Length(0, 50)
                .WithMessage("Длина поля не должна превышать 50 символов");       
        }


        /// <summary>
        /// Определяет, соответствует ли имя пользователя условиям.
        /// Имя пользователя:
        /// Должно быть длиной от 2 до 50 символов,
        /// Должно начинаться с буквы A-Za-Z,
        /// Может содержать буквы, цифры или '.','-','_',@
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <returns>True, если корректный</returns>
        private bool CheckUserName(string login)
        {
            if (login == null)
                return false;

            string pattern = @"^[a-zA-Z][a-zA-Z0-9\-_.@]{1,49}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(login);
        }

        /// <summary>
        /// Проверка текстовых полей. Поле может состоять из символов руского, английского алфавита и пробелов
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool CheckFieldCharacter(string field)
        {
            if (field == null)
            {
                return true;
            }

            string pattern = @"^[a-zA-Zа-яёА-ЯЁ\s]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(field);
        }
    }
}