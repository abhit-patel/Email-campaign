using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Commands
{
    public class UpdateRolePermissionCommand : IRequest<UpdateRolePermissionCommand>
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public List<Domain.Entities.ViewModel.PermissionListVM> PermissionList { get; set; }
    }
    
}
