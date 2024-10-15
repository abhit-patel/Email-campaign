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
    public class GetUserByRoleIDQueryHandler : IRequestHandler<GetUserByRoleIDQuery, Domain.Entities.User>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetUserByRoleIDQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.User> Handle(GetUserByRoleIDQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.RoleId == request.roleId);
        }
    }
}
