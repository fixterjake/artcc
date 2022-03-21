using FluentValidation;

namespace ZDC.Server.Validators;

public class FileValidator : AbstractValidator<Shared.Models.File>
{
    public FileValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.UploadId).NotEmpty();
    }
}
