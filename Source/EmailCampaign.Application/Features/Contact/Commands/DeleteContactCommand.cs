using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Contact.Commands
{
    public class DeleteContactCommand : IRequest<Domain.Entities.Contact>
    {
        public Guid Id { get; set; }
    }
}
