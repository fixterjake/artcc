using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class OtsValidator : AbstractValidator<Ots>
{
    public OtsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RecommenderId).NotEmpty();
    }
}