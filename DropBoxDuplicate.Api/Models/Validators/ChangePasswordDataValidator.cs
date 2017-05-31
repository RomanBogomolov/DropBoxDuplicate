using FluentValidation;

namespace DropBoxDuplicate.Api.Models.Validators
{
    public class ChangePasswordDataValidator : AbstractValidator<ChangePasswordData>
    {
        public ChangePasswordDataValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Старый пароль не может быть пустым.")
                .Length(6, 30)
                .WithMessage("Старый пароль не может быть меньше 6 и больше 30 символов.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Новый пароль не может быть пустым.")
                .Length(6, 30)
                .WithMessage("Новый пароль не может быть меньше 6 и больше 30 символов.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Пароли не совпадают.");
        }
    }
}