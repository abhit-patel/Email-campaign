using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
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
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Role>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public DeleteRoleCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }
        public async Task<Role> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (role == null)
            {
                return new Role();
            }

            role.IsDeleted = true;
            role.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            role.UpdatedOn = DateTime.Now;

            _dbContext.Entry(role).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return role;
        }
    }
}
