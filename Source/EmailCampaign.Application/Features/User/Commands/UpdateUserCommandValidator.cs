using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Commands
{
    public class UpdateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(command => command.FirstName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.LastName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.BirthDate)
            .NotEmpty();

            RuleFor(command => command.Password)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.RoleId)
            .NotEmpty();
        }
    }
}
