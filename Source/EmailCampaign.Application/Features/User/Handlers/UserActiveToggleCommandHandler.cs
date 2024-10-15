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
    public class UserActiveToggleCommandHandler : IRequestHandler<ContactActiveToggleCommand, Domain.Entities.User>
    {

        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;

        public UserActiveToggleCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.User> Handle(ContactActiveToggleCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == request.email);

            if (user != null)
            {
                if (user.IsActive)
                {
                    user.IsActive = false;
                }
                else
                {
                    user.IsActive = true;
                }
            }

            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(user).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User IsActive toggle event occur.",
                    Body = "User IsActive status is now " + user.IsActive + "." + " and it's updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/User"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;
        }
    }
}

