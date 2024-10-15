using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Commands
{
    public class CreateRolePermissionCommand : IRequest<Domain.Entities.ViewModel.RolePermissionVM>
    {
        public string RoleName { get; set; }
        public List<Domain.Entities.ViewModel.PermissionListVM> PermissionList { get; set; }
    }
}
