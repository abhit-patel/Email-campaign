using EmailCampaign.Application.Features.Permission.Commands;
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
    public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, Domain.Entities.Permission>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;
        public DeletePermissionCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.Permission> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (permission != null)
            {

                permission.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                permission.UpdatedOn = DateTime.UtcNow;
                permission.IsDeleted = true;

                _dbContext.Entry(permission).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return permission;
            }
            return new Domain.Entities.Permission();
        }
    }
}
