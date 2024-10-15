using EmailCampaign.Application.Features.Contact.Commands;
using EmailCampaign.Application.Features.Permission.Commands;
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
    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Domain.Entities.Contact>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;
        public DeleteContactCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.Contact> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (contact != null)
            {

                contact.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                contact.UpdatedOn = DateTime.UtcNow;
                contact.IsDeleted = true;

                _dbContext.Entry(contact).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();

                return contact;
            }
            return new Domain.Entities.Contact();
        }
    }
}
