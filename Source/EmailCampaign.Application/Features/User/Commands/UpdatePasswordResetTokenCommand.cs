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

namespace EmailCampaign.Application.Features.User.Commands
{
    public class UpdatePasswordResetTokenCommand : IRequest<Domain.Entities.User>
    {
        public string email { get; set; }
        public string token { get; set; }

        public class UpdatePasswordResetTokenCommandHandler : IRequestHandler<UpdatePasswordResetTokenCommand, Domain.Entities.User>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IUserContextService _userContextService;
            private readonly INotificationRepository _notificationRepository;

            public UpdatePasswordResetTokenCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
            {
                _dbContext = dbContext;
                _userContextService = userContextService;
                _notificationRepository = notificationRepository;
            }
            public async Task<Domain.Entities.User> Handle(UpdatePasswordResetTokenCommand request, CancellationToken cancellationToken)
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == request.email);


                if (user != null)
                {
                    user.ResetpasswordCode = request.token;

                    user.UpdatedOn = DateTime.UtcNow;
                }
                else
                {
                    return new Domain.Entities.User();
                }

                _dbContext.Entry(user).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();

                if (user != null)
                {
                    var notification = new Notification
                    {
                        Header = "User request for forgot passowrd",
                        Body = "ForgotPassword token generated for user " + user.FirstName + " " + user.LastName + " ("+ user.ID  +").",
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
