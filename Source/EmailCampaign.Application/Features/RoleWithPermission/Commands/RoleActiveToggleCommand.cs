using EmailCampaign.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Commands
{
    public class RoleActiveToggleCommand : IRequest<Role>
    {
        public string name { get; set; }
    }
}
