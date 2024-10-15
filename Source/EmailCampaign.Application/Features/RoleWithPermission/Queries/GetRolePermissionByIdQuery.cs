using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Domain.Entities.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Queries
{
    public class GetRolePermissionByIdQuery : IRequest<UpdateRolePermissionCommand>
    {
        public Guid Id { get; set; }
    }
}
