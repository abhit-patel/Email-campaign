using EmailCampaign.Application.Features.User.Queries;
using EmailCampaign.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Domain.Entities.User>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetUserByIdQueryHandler( IApplicationDbContext dbContext )
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);

            return user;
        }
    }
}
