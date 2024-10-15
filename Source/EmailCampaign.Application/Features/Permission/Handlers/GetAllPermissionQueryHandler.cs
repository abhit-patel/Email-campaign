using EmailCampaign.Application.Features.Permission.Queries;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Handlers
{
    public class GetAllPermissionQueryHandler : IRequestHandler<GetAllPermissionQuery, IEnumerable<Domain.Entities.Permission>>
    {
        private readonly IApplicationDbContext _dbContext;
        
        public GetAllPermissionQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Domain.Entities.Permission>> Handle(GetAllPermissionQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Permission.Where(p => p.IsDeleted == false).ToListAsync();
        }
    }
}
