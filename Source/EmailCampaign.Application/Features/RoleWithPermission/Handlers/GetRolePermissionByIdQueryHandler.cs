using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Application.Features.RoleWithPermission.Queries;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Handlers
{
    public class GetRolePermissionByIdQueryHandler : IRequestHandler<GetRolePermissionByIdQuery, UpdateRolePermissionCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetRolePermissionByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UpdateRolePermissionCommand> Handle(GetRolePermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == request.Id);

            List<RolePermission> dBModelList = await _dbContext.RolePermission.Where(p => p.RoleId == request.Id && p.IsDeleted == false).ToListAsync();

            UpdateRolePermissionCommand rolePermissionModelList = dBModelList.GroupBy(r => r.RoleId).Select(g => new UpdateRolePermissionCommand
            {
                RoleId = role.Id,
                RoleName = role.Name,
                PermissionList = g.Select(p => new PermissionListVM
                {
                    PermissionId = p.PermissionId,
                    IsView = p.IsView,
                    IsAddEdit = p.IsAddEdit,
                    IsDelete = p.IsDelete
                }).ToList()
            }).FirstOrDefault();

            return rolePermissionModelList;
        }
    }
}
