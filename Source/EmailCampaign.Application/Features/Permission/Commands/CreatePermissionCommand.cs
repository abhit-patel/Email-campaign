using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Commands
{
    public class CreatePermissionCommand : IRequest<Domain.Entities.Permission>
    {
        public string PageName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Slug { get; set; }
    }
}
