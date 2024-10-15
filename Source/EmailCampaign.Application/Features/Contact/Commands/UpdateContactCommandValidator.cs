using EmailCampaign.Application.Features.Contact.Commands;
using EmailCampaign.Application.Features.Permission.Commands;
using EmailCampaign.Application.Features.User.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Commands.Commands
{
    public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactCommandValidator()
        {
            RuleFor(command => command.FirstName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.LastName)
            .NotEmpty()
            .MaximumLength(100);
            RuleFor(command => command.CompanyName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(100);
        }
    }
}
