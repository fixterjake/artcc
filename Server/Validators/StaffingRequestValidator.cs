using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class StaffingRequestValidator : AbstractValidator<StaffingRequest>
{
    public StaffingRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Affiliation).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
    }
}
