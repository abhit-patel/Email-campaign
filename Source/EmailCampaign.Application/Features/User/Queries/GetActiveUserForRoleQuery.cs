using EmailCampaign.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Queries
{
    public class GetActiveUserForRoleQuery : IRequest<List<Domain.Entities.User>>
    {
        public Guid roleId { get; set; }

        public class GetActiveUserForRoleQueryCommand : IRequestHandler<GetActiveUserForRoleQuery, List<Domain.Entities.User>>
        {
            private readonly IApplicationDbContext _dbContext;
            public GetActiveUserForRoleQueryCommand(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<List<Domain.Entities.User>> Handle(GetActiveUserForRoleQuery request, CancellationToken cancellationToken)
            {
                return await _dbContext.User.Where(p => p.RoleId == request.roleId && p.IsActive == true && p.IsDeleted == false && p.IsSuperAdmin == false).ToListAsync();
            }
        }
    }
}
