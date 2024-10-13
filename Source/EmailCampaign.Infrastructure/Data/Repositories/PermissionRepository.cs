using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly ErrorLogFilter _errorLogFilter;

        public PermissionRepository(ApplicationDbContext dbContext , IUserContextService userContextService, ErrorLogFilter errorLogFilter)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _errorLogFilter = errorLogFilter;

        }

        public async Task<List<Permission>> GetAllPermissionAsync()
        {
            return await _dbContext.Permission.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task<Permission> GetPermissionAsync(Guid id)
        {
            return await _dbContext.Permission.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }


        public async Task<Permission> CreatePermissionAsync(PermissionVM model)
        {
            Permission newPermission = new Permission
            {
                Id = Guid.NewGuid(),
                ActionName = model.ActionName,
                ControllerName = model.ControllerName,
                PageName = model.PageName,
                Slug = model.Slug,
                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            await _dbContext.Permission.AddAsync(newPermission);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            return newPermission;
        }


        public async Task<Permission> UpdatePermissionAsync(Guid id, PermissionVM model)
        {
            Permission permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.Id == id);

            if (permission == null)
            {
                return null;
            }

            permission.ActionName = model.ActionName;
            permission.ControllerName = model.ControllerName;
            permission.PageName = model.PageName;
            permission.Slug = model.Slug;

            permission.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            permission.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(permission).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            return permission;

        }

        public async Task<Permission> DeletePermissionAsync(Guid ID)
        {
            Permission permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.Id == ID);

            if (permission != null)
            {

                permission.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                permission.UpdatedOn = DateTime.UtcNow;
                permission.IsDeleted = true;

                _dbContext.Entry(permission).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return permission;
                }
                catch (DbUpdateException ex)
                {
                    await _errorLogFilter.OnException(ex);
                }
            }
            return new Permission();
        }

        public async Task<List<SelectListItem>> GetPermissionAsSelectListItemsAsync()
        {
            return await _dbContext.Permission.Where(p => p.IsDeleted == false)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ControllerName
                })
                .ToListAsync();
        }
    }
}
