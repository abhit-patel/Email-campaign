using EmailCampaign.Application.Features.Contact.Queries;
using EmailCampaign.Application.Features.Permission.Queries;
using EmailCampaign.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Contact.Handlers
{
    public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, Domain.Entities.Contact>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetContactByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.Contact> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false);
        }
    }
}
