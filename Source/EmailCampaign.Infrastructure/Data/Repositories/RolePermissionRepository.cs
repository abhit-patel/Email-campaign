using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        public RolePermissionRepository( ApplicationDbContext dbContext , IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public async Task<List<RolePermission>> GetAllAsync()
        {
            return await _dbContext.RolePermission.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task<RolePermission> GetByIdAsync(Guid id)
        {
            return await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<RolePermission> CreateAsync(RolePermissionDBVM model)
        {
            RolePermission newItem = new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = model.RoleId,
                PermissionId = model.PermissionId,
                IsAddEdit = model.IsAddEdit,
                IsDelete = model.IsDelete,
                IsView = model.IsView,
                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            await _dbContext.RolePermission.AddAsync(newItem);

            await _dbContext.SaveChangesAsync();

            return newItem;
        }


        public async Task<RolePermission> UpdateAsync(Guid id, RolePermissionDBVM model)
        {
            RolePermission rolePermission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.Id == id);

            if ( rolePermission == null) 
            {
                return null;
            }

            rolePermission.RoleId = model.RoleId;
            rolePermission.PermissionId = model.PermissionId;
            rolePermission.IsView = model.IsView;
            rolePermission.IsAddEdit = model.IsAddEdit;
            rolePermission.IsDelete = model.IsDelete;

            rolePermission.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            rolePermission.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(rolePermission).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Error updating user with ID {id}", id);
                throw;
            }

            return rolePermission;
        }


        public async Task<bool> DeleteAsync(Guid ID)
        {
            RolePermission rolePermission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.Id == ID);

            if (rolePermission != null)
            {

                rolePermission.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                rolePermission.UpdatedOn = DateTime.UtcNow;
                rolePermission.IsDelete = true;

                _dbContext.Entry(rolePermission).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
                return false; 
        }

    }
}
