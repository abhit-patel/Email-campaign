using EmailCampaign.Application.Features.RoleWithPermission.Queries;
using EmailCampaign.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace EmailCampaign.Application.Features.RoleWithPermission.Handlers
{
    public class GetRoleAsSelectListItemQueryHandler : IRequestHandler<GetRoleAsSelectListItemQuery, List<SelectListItem>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetRoleAsSelectListItemQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<SelectListItem>> Handle(GetRoleAsSelectListItemQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Role.Where(p => p.IsDeleted == false)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToListAsync();
        }
    }
}
