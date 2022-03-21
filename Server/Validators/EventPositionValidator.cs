using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class EventPositionValidator : AbstractValidator<EventPosition>
{
    public EventPositionValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Rating).NotEmpty();
    }
}
