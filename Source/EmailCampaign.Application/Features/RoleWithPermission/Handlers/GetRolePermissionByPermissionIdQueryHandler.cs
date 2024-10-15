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
    public class GetRolePermissionByPermissionIdQueryHandler : IRequestHandler<GetRolePermissionByPermissionIdQuery, RolePermission>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetRolePermissionByPermissionIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<RolePermission> Handle(GetRolePermissionByPermissionIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.PermissionId == request.permissionId && p.IsDeleted == false);
        }
    }
}
