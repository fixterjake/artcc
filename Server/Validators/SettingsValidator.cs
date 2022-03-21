using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class SettingsValidator : AbstractValidator<Settings>
{
    public SettingsValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
    }
}
