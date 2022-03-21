using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class LoaValidator : AbstractValidator<Loa>
{
    public LoaValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}
