using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class NewsValidator : AbstractValidator<News>
{
    public NewsValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
