using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Queries
{
    public class GetAllPermissionQuery : IRequest<IEnumerable<Domain.Entities.Permission>>
    {

    }
}
