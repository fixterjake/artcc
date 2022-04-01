using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class TrainingTicketValidator : AbstractValidator<TrainingTicket>
{
    public TrainingTicketValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TrainerId).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
        RuleFor(x => x.Comments).NotEmpty();
        RuleFor(x => x.InternalComments).NotEmpty();
    }
}
