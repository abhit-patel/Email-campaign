using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities.ViewModel;
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

namespace EmailCampaign.Application.Features.User.Commands
{
    public class ResetPasswordCommand : IRequest<Domain.Entities.User>
    {
        public string email { get; set; }
        public string password { get; set; }

        public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Domain.Entities.User>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IUserContextService _userContextService;
            private readonly INotificationRepository _notificationRepository;
            private readonly PasswordHasher _passwordHasher;

            public ResetPasswordCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository, PasswordHasher passwordHasher)
            {
                _dbContext = dbContext;
                _userContextService = userContextService;
                _notificationRepository = notificationRepository;
                _passwordHasher = passwordHasher;
            }
            public async Task<Domain.Entities.User> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == request.email);

                if (user != null)
                {
                    return new Domain.Entities.User();
                }
                HashedPassVM items = _passwordHasher.HashPassword(request.password);

                user.Password = request.password;
                user.HashPassword = items.hashedPassword;
                user.SaltKey = items.saltKey;


                user.UpdatedBy = user.ID;
                user.UpdatedOn = DateTime.UtcNow;

                _dbContext.Entry(user).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();


                if (user != null)
                {
                    var notification = new Notification
                    {
                        Header = "Reset password by mail",
                        Body = "User " + user.FirstName + " " + user.LastName + " updated account's password by " + user.Email + ".",
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
}
