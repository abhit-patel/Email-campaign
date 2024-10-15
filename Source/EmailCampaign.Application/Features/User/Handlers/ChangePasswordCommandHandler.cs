using EmailCampaign.Application.Features.User.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
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
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Domain.Entities.User>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserContextService _userContextService;
        private readonly IApplicationDbContext _dbContext;
        private readonly PasswordHasher _passwordHasher;

        public ChangePasswordCommandHandler(INotificationRepository notificationRepository, IUserContextService userContextService, IApplicationDbContext dbContext, PasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
            _userContextService = userContextService;
            _passwordHasher = passwordHasher;

        }
        public async Task<Domain.Entities.User> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(_userContextService.GetUserId());

            var user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == userId);

            if (user == null)
            {
                return new Domain.Entities.User();
            }

            //for get hashed new password
            HashedPassVM items = _passwordHasher.HashPassword(request.Password);

            user.Password = request.Password;
            user.HashPassword = items.hashedPassword;
            user.SaltKey = items.saltKey;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = userId;

            _dbContext.Entry(user).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "Reset password from profile",
                    Body = "User " + user.FirstName + " " + user.LastName + " updated account's password from profile tab.",
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
