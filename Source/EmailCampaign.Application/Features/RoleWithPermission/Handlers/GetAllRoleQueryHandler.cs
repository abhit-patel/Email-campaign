using EmailCampaign.Application.Features.RoleWithPermission.Queries;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Handlers
{
    public class GetAllRoleQueryHandler : IRequestHandler<GetAllRoleQuery, IEnumerable<Role>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetAllRoleQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Role>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Role.Where(p => p.IsDeleted == false).ToListAsync(cancellationToken);
        }
    }
}
