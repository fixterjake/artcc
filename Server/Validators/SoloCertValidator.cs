﻿using FluentValidation;
using ZDC.Shared.Models;

namespace ZDC.Server.Validators;

public class SoloCertValidator : AbstractValidator<SoloCert>
{
    public SoloCertValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SubmitterId).NotEmpty();
        RuleFor(x => x.Position).NotEmpty();
        RuleFor(x => x.OldCert).NotEmpty();
        RuleFor(x => x.Cert).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
    }
}
