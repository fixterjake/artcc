using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class PositionValidator : AbstractValidator<Position>
{
    public PositionValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
