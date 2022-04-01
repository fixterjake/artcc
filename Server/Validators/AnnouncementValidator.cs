using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class AnnouncementValidator : AbstractValidator<Announcement>
{
    public AnnouncementValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
