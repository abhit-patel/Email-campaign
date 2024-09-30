using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Query.QueryService
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionType { get; }

        public PermissionRequirement(string permissionType)
        {
            PermissionType = permissionType;
        }
    }
}
