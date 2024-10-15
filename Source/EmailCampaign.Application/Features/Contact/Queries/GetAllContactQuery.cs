using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Contact.Queries
{
    public class GetAllContactQuery : IRequest<IEnumerable<Domain.Entities.Contact>>
    {

    }
}
