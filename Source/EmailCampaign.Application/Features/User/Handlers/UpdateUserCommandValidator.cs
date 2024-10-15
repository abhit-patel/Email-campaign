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
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Domain.Entities.User>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;

        public UpdateUserCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }

        public async Task<Domain.Entities.User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == request.Id);


            if (user != null)
            {

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.BirthDate = request.BirthDate;
                user.IsActive = request.IsActive;
                user.RoleId = request.RoleId;

                user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                user.UpdatedOn = DateTime.UtcNow;

            }

            _dbContext.Entry(user).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User Details updated",
                    Body = "Your details updated by " + _userContextService.GetUserName() + ".",
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