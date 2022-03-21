using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class VisitRequestValidator : AbstractValidator<VisitRequest>
{
    public VisitRequestValidator()
    {
        RuleFor(x => x.Cid).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Rating).NotEmpty();
        RuleFor(x => x.VisitReason).NotEmpty();
    }
}
