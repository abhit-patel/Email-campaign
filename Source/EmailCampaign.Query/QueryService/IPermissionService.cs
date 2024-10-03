using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Query.QueryService
{
    public interface IPermissionService
    {
        Task<RolePermission> GetRolePermissionAsync(Guid userId, string controllerName, string actionName);
    }
}
