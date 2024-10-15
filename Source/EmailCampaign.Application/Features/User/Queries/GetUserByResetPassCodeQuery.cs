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
    public class GetUserByResetPassCodeQuery : IRequest<Domain.Entities.User>
    {
        public string token { get; set; }

        public class GetUserByResetPassCodeQueryHandler : IRequestHandler<GetUserByResetPassCodeQuery, Domain.Entities.User>
        {
            private readonly IApplicationDbContext _dbContext;
            public GetUserByResetPassCodeQueryHandler(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Domain.Entities.User> Handle(GetUserByResetPassCodeQuery request, CancellationToken cancellationToken)
            {
                return await _dbContext.User.FirstOrDefaultAsync(p => p.IsDeleted == false && p.ResetpasswordCode == request.token);
            }
        }
    }
}
