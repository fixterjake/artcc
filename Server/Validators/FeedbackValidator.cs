using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class FeedbackValidator : AbstractValidator<Feedback>
{
    public FeedbackValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Callsign).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Cid).NotEmpty();
    }
}
