using EmailCampaign.Application.Features.Contact.Commands;
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

namespace EmailCampaign.Application.Features.Contact.Handlers
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Domain.Entities.Contact>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;
        public CreateContactCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
        }
        public async Task<Domain.Entities.Contact> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = new Domain.Entities.Contact
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CompanyName = request.CompanyName,
                IsActive = request.IsActive,

                IsDeleted = false,

                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
            };

            await _dbContext.Contact.AddAsync(contact);

            await _dbContext.SaveChangesAsync();

            return contact;
        }
    }
}
