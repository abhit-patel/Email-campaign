using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace EmailCampaign.Application.Features.RoleWithPermission.Queries
{
    public class GetPermissionAsSelectListItemQuery : IRequest<List<SelectListItem>>
    {
    }
}
