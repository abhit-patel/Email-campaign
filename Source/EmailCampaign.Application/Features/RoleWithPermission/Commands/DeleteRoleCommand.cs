using EmailCampaign.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Commands
{
    public class DeleteRoleCommand : IRequest<Role>
    {
        public Guid Id { get; set; }
    }
}
