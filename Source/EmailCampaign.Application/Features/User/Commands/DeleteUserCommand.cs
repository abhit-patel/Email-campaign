using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Commands
{
    public class DeleteUserCommand : IRequest<Domain.Entities.User>
    {
        public Guid Id { get; set; }
    }
}
