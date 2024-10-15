using EmailCampaign.Domain.Entities.ViewModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Commands
{
    public class CreateRolePermissionCommandValidator : AbstractValidator<CreateRolePermissionCommand>
    {
        public CreateRolePermissionCommandValidator()
        {
            RuleFor(command => command.RoleName)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(x => x.PermissionList)
            .Must(HaveAtLeastOnePermission)
            .WithMessage("At least one permission must be selected for each item.");

        }

        private bool HaveAtLeastOnePermission(List<PermissionListVM> permissionList)
        {
            // Check if at least one permission has one of the properties as true
            return permissionList != null && permissionList.Any(p => p.IsView || p.IsAddEdit || p.IsDelete);
        }
    }



    public class PermissionListVMValidator : AbstractValidator<Domain.Entities.ViewModel.PermissionListVM>
    {
        public PermissionListVMValidator()
        {
            RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("Permission ID is required");

            RuleFor(x => x.IsView)
            .NotNull().WithMessage("IsView must be specified");

            RuleFor(x => x.IsAddEdit)
            .NotNull().WithMessage("IsAddEdit must be specified");

            RuleFor(x => x.IsDelete)
            .NotNull().WithMessage("IsDelete must be specified");
        }
    }
}
