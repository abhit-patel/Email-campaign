using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Queries
{
    public class GetUserByIdQuery : IRequest<Domain.Entities.User>
    {
        public Guid Id { get; set; }
    }
}
