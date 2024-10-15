using EmailCampaign.Application.Features.User.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Commands
{
    public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
            RuleFor(command => command.PageName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.ActionName)
            .NotEmpty()
            .MaximumLength(100);
            RuleFor(command => command.ControllerName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(command => command.Slug)
            .NotEmpty()
            .MaximumLength(100);
        }
    }
}
