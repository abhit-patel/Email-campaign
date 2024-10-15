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
    public class GetAllRolePermissionQueryHandler : IRequestHandler<GetAllRolePermissionQuery, IEnumerable<RolePermission>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetAllRolePermissionQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<RolePermission>> Handle(GetAllRolePermissionQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.RolePermission.Where(p => p.IsDeleted == false).ToListAsync();
        }
    }
}
