using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Commands
{
    public class UpdateActiveToggleCommand : IRequest<Domain.Entities.User>
    {
        public string email { get; set; }
    }
}
