using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class AirportValidator : AbstractValidator<Airport>
{
    public AirportValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Icao).NotEmpty();
        RuleFor(x => x.Latitude).NotEmpty();
        RuleFor(x => x.Longitude).NotEmpty();
    }
}
