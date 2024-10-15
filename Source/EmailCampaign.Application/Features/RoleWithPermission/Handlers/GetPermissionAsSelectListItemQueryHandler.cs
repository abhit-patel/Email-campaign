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
    public class GetPermissionAsSelectListItemQueryHandler : IRequestHandler<GetPermissionAsSelectListItemQuery, List<SelectListItem>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetPermissionAsSelectListItemQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<SelectListItem>> Handle(GetPermissionAsSelectListItemQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Permission.Where(p => p.IsDeleted == false)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ControllerName
                })
                .ToListAsync();
        }
    }
}
