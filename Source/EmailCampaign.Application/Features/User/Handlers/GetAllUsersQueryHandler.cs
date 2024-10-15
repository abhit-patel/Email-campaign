using EmailCampaign.Application.Features.User.Queries;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUserQuery, IEnumerable<Domain.Entities.User>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetAllUsersQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Domain.Entities.User>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.User.Where(p => p.IsDeleted == false && p.IsSuperAdmin == false).ToListAsync(cancellationToken);
        }
    }
}
