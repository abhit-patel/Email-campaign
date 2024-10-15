using EmailCampaign.Application.Features.RoleWithPermission.Queries;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Handlers
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Role>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetRoleByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Role> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            // review this line for correct use this cancellation token.
            return await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == request.Id).WaitAsync(cancellationToken);
        }
    }
}
