using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Commands
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(command => command.Password)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.ConfirmPassword)
            .NotEmpty()
            .MaximumLength(100);
        }
    }
}
