using EmailCampaign.Domain.Entities;
using EmailCampaign.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Query.QueryService
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _dbContext;

        public PermissionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RolePermission> GetRolePermissionAsync(Guid userId, string controllerName, string actionName)
        {

            var roleID = _dbContext.User.Where(p => p.ID == userId && p.IsDeleted == false).Select(p => p.RoleId).SingleOrDefault();

            var roleName = await _dbContext.Role.FindAsync(Guid.Parse(roleID.ToString().ToUpper()));
            if (roleName == null) return null;

            var rolePermission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.RoleId == Guid.Parse(roleID.ToString().ToUpper()) && p.Permission.ControllerName == controllerName && p.IsDeleted == false);

            return rolePermission;
        }
    }
}
