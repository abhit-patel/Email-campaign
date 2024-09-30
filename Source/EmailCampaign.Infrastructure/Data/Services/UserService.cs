using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailCampaign;
using EmailCampaign.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EmailCampaign.Infrastructure.Data.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            Guid roleId = await _dbContext.User.Where(p => p.ID == userId).Select(p => p.RoleId).SingleOrDefaultAsync();

            return await _dbContext.Role.Where(p => p.Id == roleId).Select(p => p.Name).ToListAsync();

        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
        {
            Guid roleId = await _dbContext.User.Where(p => p.ID == userId).Select(p => p.RoleId).SingleOrDefaultAsync();

            List<Guid> permissionId = await _dbContext.RolePermission.Where(p => p.RoleId == roleId).Select(p => p.PermissionId).ToListAsync();

            return await _dbContext.Permission.Where(p => permissionId.Contains(p.Id)).Select(p => p.Slug).Distinct().ToListAsync();
        }
    }
}
