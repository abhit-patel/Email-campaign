using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Services
{
    public class PermissionService : IPermissionService
    {
        public bool HasPermission(ClaimsPrincipal user, string permission)
        {
            return user.HasClaim("Permission", permission);
        }
    }
}
