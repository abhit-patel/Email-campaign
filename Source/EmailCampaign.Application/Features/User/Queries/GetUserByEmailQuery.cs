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
    public class GetUserByEmailQuery : IRequest<Domain.Entities.User>
    {
        public string email { get; set; }

        public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Domain.Entities.User>
        {
            private readonly IApplicationDbContext _dbContext;
            public GetUserByEmailQueryHandler(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Domain.Entities.User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
            {
                return await _dbContext.User.FirstOrDefaultAsync(p => p.Email == request.email && p.IsActive == true && p.IsDeleted == false);
            }
        }
    }
}
