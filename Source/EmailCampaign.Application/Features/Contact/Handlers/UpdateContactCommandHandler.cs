using EmailCampaign.Application.Features.Contact.Commands;
using EmailCampaign.Application.Features.Permission.Commands;
using EmailCampaign.Application.Features.User.Commands;
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

namespace EmailCampaign.Application.Features.Contact.Handlers
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Domain.Entities.Contact>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;
        public UpdateContactCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.Contact> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (contact == null)
            {
                return new Domain.Entities.Contact();
            }

            contact.FirstName = request.FirstName;
            contact.LastName = request.LastName;
            contact.CompanyName = request.CompanyName;
            contact.Email = request.Email;
            contact.IsActive = request.IsActive;

            contact.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            contact.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(contact).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return contact;
        }
    }
}
