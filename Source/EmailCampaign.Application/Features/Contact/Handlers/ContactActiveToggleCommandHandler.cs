using EmailCampaign.Application.Features.Contact.Commands;
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

namespace EmailCampaign.Application.Features.Contact.Handlers
{
    public class ContactActiveToggleCommandHandler : IRequestHandler<ContactActiveToggleCommand, Domain.Entities.Contact>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;

        public ContactActiveToggleCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.Contact> Handle(ContactActiveToggleCommand request, CancellationToken cancellationToken)
        {
            var contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Email == request.email);

            if (contact != null)
            {
                if (contact.IsActive)
                {
                    contact.IsActive = false;
                }
                else
                {
                    contact.IsActive = true;
                }
            }

            contact.UpdatedOn = DateTime.UtcNow;
            contact.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(contact).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            if (contact != null)
            {
                var notification = new Notification
                {
                    Header = "User IsActive toggle event occur.",
                    Body = "User IsActive status is now " + contact.IsActive + "." + " and it's updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = contact.CreatedBy,
                    PerformOperationFor = contact.Id,
                    RedirectUrl = "/User"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return contact;
        
    }
    }
}
