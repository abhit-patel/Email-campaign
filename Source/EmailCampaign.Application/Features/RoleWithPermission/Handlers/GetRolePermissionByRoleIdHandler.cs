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
    public class GetRolePermissionByRoleIdHandler : IRequestHandler<GetRolePermissionByRoleIdQuery, RolePermission>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetRolePermissionByRoleIdHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<RolePermission> Handle(GetRolePermissionByRoleIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.RoleId == request.roleId && p.IsDelete == false);
        }
    }
}
