using AutoMapper;
using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Handlers
{
    public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand, UpdateRolePermissionCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        public UpdateRolePermissionCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService , IMapper mapper )
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }
        public async Task<UpdateRolePermissionCommand> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
        {
            var roleItem = await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == request.RoleId);

            if(roleItem == null)
            {
                return new UpdateRolePermissionCommand();
            }

            roleItem.Name = request.RoleName;
            roleItem.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            roleItem.UpdatedOn = DateTime.Now;
            
            _dbContext.Entry(roleItem).State = EntityState.Modified;
                

            foreach (var permissions in request.PermissionList)
            {
                var rolePermission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.RoleId == request.RoleId && p.PermissionId == permissions.PermissionId);

                if(rolePermission == null)
                {
                    return new UpdateRolePermissionCommand();
                }

                rolePermission.IsView = permissions.IsView;
                rolePermission.IsAddEdit = permissions.IsAddEdit;
                rolePermission.IsDelete = permissions.IsDelete;

                rolePermission.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                rolePermission.UpdatedOn = DateTime.UtcNow;

                _dbContext.Entry(rolePermission).State = EntityState.Modified;
                
            }

            await _dbContext.SaveChangesAsync();

            return request;
        }

    }
}
