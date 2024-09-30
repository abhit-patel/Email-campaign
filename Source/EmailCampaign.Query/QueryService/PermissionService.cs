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
            var roleID =  _dbContext.User.Where(p => p.ID == userId).Select(p => p.RoleId).ToString();
            var roleName = await _dbContext.Role.FindAsync(Guid.Parse(roleID));
            if (roleName == null) return null;


            var permission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.RoleId == Guid.Parse(roleID) && p.Permission.ActionName == actionName);

            return permission;
        }

        public async Task<bool> HasPermission(Guid roleId, string controller, string actionName, string permissionType)
        {
            var permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.ControllerName == controller && p.ActionName == actionName);

            if (permission == null) { return  false; }

            var rolePermission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.RoleId == roleId && p.PermissionId == permission.Id);

            if (rolePermission == null) { return false;}

            return permissionType switch
            {
                "View" => rolePermission.IsView,
                "Edit" => rolePermission.IsAddEdit,
                "Delete" => rolePermission.IsDelete,
                _ => false
            };
        }
    }
}
