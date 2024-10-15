using EmailCampaign.Application.Features.User.Queries;
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

namespace EmailCampaign.Application.Features.User.Handlers
{
    public class GetUserInfoForProfileQueryHandler : IRequestHandler<GetUserInfoForProfileQuery, Domain.Entities.User>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IAuthRepository _authRepository;

        public GetUserInfoForProfileQueryHandler(IAuthRepository authRepository, IUserContextService userContextService, IApplicationDbContext dbContext)
        {
            _authRepository = authRepository;
            _userContextService = userContextService;
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.User> Handle(GetUserInfoForProfileQuery request, CancellationToken cancellationToken)
        {
            Guid UserId = Guid.Parse(_userContextService.GetUserId());

            var user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == UserId && p.IsDeleted == false);

            if (user == null)
            {
                return new Domain.Entities.User();
            }

            return user;
        }
    }
}
