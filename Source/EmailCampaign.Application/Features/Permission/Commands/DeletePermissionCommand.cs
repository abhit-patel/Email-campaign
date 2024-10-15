using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Commands
{
    public class DeletePermissionCommand : IRequest<Domain.Entities.Permission>
    {
        public Guid Id { get; set; }
    }
}
