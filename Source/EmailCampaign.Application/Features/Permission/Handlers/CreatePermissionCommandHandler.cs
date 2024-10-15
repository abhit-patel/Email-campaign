using EmailCampaign.Application.Features.Permission.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Permission.Handlers
{
    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, Domain.Entities.Permission>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;
        public CreatePermissionCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.Permission> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var newPermission = new Domain.Entities.Permission
            {
                Id = Guid.NewGuid(),
                ActionName = request.ActionName,
                ControllerName = request.ControllerName,
                PageName = request.PageName,
                Slug = request.Slug,
                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            await _dbContext.Permission.AddAsync(newPermission);

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (newPermission != null)
            {
                var notification = new Notification
                {
                    Header = "New Permission created.",
                    Body = "User " + _userContextService.GetUserName() + " Created new Permission with " + newPermission.ControllerName + " (" + newPermission.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Permission"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return newPermission;
        }
    }
}
