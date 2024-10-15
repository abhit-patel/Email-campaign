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
    public class CheckRegisteredEmailQueryHandler : IRequestHandler<CheckRegisteredEmailQuery, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        public CheckRegisteredEmailQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Handle(CheckRegisteredEmailQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.User.AnyAsync(p => p.Email.ToUpper() == request.email.ToUpper() && p.IsDeleted == false);
        }
    }
}
