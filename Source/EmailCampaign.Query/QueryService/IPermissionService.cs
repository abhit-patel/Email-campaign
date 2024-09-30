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
        Task<bool> HasPermission(Guid roleId, string controller, string action, string permissionType);

        Task<RolePermission> GetRolePermissionAsync(Guid userId, string controllerName, string actionName);
    }
}
