using FluentValidation;

namespace DropBoxDuplicate.Model.Validators
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(x => x.Text)
                .Length(1, 256)
                .WithMessage("Необходимо указать текст.");
        }
    }
}