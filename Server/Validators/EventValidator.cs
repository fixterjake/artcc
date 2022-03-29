using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Host).NotEmpty();
        RuleFor(x => x.UploadId).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
    }
}
