using EmailCampaign.Application.Features.User.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Contact.Commands
{
    public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactCommandValidator()
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
