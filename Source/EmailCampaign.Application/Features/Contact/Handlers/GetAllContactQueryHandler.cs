using EmailCampaign.Application.Features.Contact.Queries;
using EmailCampaign.Application.Features.Permission.Queries;
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
    public class GetAllContactQueryHandler : IRequestHandler<GetAllContactQuery, IEnumerable<Domain.Entities.Contact>>
    {
        private readonly IApplicationDbContext _dbContext;
        
        public GetAllContactQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Domain.Entities.Contact>> Handle(GetAllContactQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Contact.Where(p => p.IsDeleted == false).ToListAsync();
        }
    }
}
