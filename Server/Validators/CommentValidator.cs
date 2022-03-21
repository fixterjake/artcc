using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SubmitterId).NotEmpty();
    }
}
