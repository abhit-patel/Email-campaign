using EmailCampaign.Application.Features.Permission.Queries;
using EmailCampaign.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Handlers
{
    public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, Domain.Entities.Permission>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetPermissionByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.Permission> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Permission.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false);
        }
    }
}
