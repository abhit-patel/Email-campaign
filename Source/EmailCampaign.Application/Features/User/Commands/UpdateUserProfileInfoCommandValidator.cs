using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmailCampaign.Application.Features.User.Commands
{
    public class UpdateUserProfileInfoCommandValidator : AbstractValidator<UpdateUserProfileInfoCommand>
    {
        public UpdateUserProfileInfoCommandValidator()
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
            
            RuleFor(command => command.Birthdate)
           .NotEmpty();

        }
    }
}
