using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class EventRegistrationValidator : AbstractValidator<EventRegistration>
{
    public EventRegistrationValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PositionId).NotEmpty();
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
    }
}
