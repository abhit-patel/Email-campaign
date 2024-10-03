using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        public RoleRepository(ApplicationDbContext dbContext , IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }


        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await _dbContext.Role.Where(p => p.IsDeleted == false).ToListAsync() ;
        }

        public async Task<Role> GetRoleByIdAsync(Guid id)
        {
            return await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await _dbContext.Role.FirstOrDefaultAsync(p => p.Name == name && p.IsDeleted == false);
        }

        public async Task<Role> CreateRoleAsync(string roleName)
        {
            Role newRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            await _dbContext.Role.AddAsync(newRole);

            await _dbContext.SaveChangesAsync();

            return newRole;
        }


        public async Task<Role> UpdateRoleAsync(Guid id, RoleVM model)
        {

            Role role = await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == id);

            if (role == null) { return null; }

            role.Name = model.RoleName;

            role.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            role.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(role).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return role;
        }

        public async Task<bool> DeleteRoleAsync(Guid ID)
        {
            Role role = await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == ID);

            if (role != null)
            {

                role.UpdatedBy = Guid.NewGuid();
                role.UpdatedOn = DateTime.UtcNow;
                role.IsDeleted = true;

                _dbContext.Entry(role).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
            return false;
        }


        public async Task<List<SelectListItem>> GetRolesAsSelectListItemsAsync()
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
