using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
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
    public class RoleActiveToggleCommandHandler : IRequestHandler<RoleActiveToggleCommand, Role>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;

        public RoleActiveToggleCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Role> Handle(RoleActiveToggleCommand request, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Role.FirstOrDefaultAsync(p => p.Name.ToUpper() == request.name.ToUpper());

            if (role != null)
            {
                if (role.IsActive)
                {
                    role.IsActive = false;
                }
                else
                {
                    role.IsActive = true;
                }
            }
            else
            {
                return new Role();
            }

            role.UpdatedOn = DateTime.UtcNow;
            role.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(role).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            if (role != null)
            {
                var notification = new Notification
                {
                    Header = "Role IsActive toggle event occur.",
                    Body = "Role IsActive status is now " + role.IsActive + "." + " and it's updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = role.CreatedBy,
                    PerformOperationFor = role.Id,
                    RedirectUrl = "/User"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return role;
        }
    }
}
