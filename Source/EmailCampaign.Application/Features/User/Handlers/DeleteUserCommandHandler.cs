using EmailCampaign.Application.Features.User.Commands;
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

namespace EmailCampaign.Application.Features.User.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Domain.Entities.User>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;

        public DeleteUserCommandHandler(IApplicationDbContext dbContext, IUserContextService userContext, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContext;
            _notificationRepository = notificationRepository;
        }

        public async Task<Domain.Entities.User> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == request.Id);


            if (user != null)
            {

                user.IsDeleted = true;

                user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                user.UpdatedOn = DateTime.UtcNow;

            }

            _dbContext.Entry(user).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User deleted",
                    Body = "user " + user.FirstName + " " + user.LastName + " (" + user.ID + ") deleted. user deleted by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/Notification"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;
        }
    }
}