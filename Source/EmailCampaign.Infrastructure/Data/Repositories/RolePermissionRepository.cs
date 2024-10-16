﻿using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Services;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
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
        private readonly ErrorLogFilter _errorLogFilter;


        public RolePermissionRepository( ApplicationDbContext dbContext , IUserContextService userContextService , ErrorLogFilter errorLogFilter)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _errorLogFilter = errorLogFilter;
        }

        public async Task<List<RolePermission>> GetAllAsync()
        {
            return await _dbContext.RolePermission.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task<RolePermission> GetByIdAsync(Guid id)
        {
            return await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task<RolePermission> GetItemByRoleID(Guid roleId)
        {
            return await _dbContext.RolePermission.FirstOrDefaultAsync(p =>p.RoleId == roleId);
        }

        public async Task<RolePermission> GetItemByPermissionId(Guid permissionId)
        {
            return await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        }


        public async Task<RolePermissionVM> GetAllByIdAsync(Guid id)
        {
            //RolePermissionVM model = new RolePermissionVM();

            var roleName = await _dbContext.Role.Where(p => p.Id == id).Select(p => p.Name).SingleOrDefaultAsync();

            List<RolePermission> dBModelList = await _dbContext.RolePermission.Where(p => p.RoleId == id && p.IsDeleted == false).ToListAsync();

            RolePermissionVM rolePermissionVMList = dBModelList.GroupBy(r => r.RoleId).Select(g => new RolePermissionVM
            {
                RoleName = roleName,
                PermissionList = g.Select(p => new PermissionListVM
                {
                    PermissionId = p.PermissionId,
                    IsView = p.IsView,
                    IsAddEdit = p.IsAddEdit,
                    IsDelete = p.IsDelete
                }).ToList()
            }).FirstOrDefault();

            return rolePermissionVMList;

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

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            return newItem;
        }


        public async Task<RolePermission> UpdateAsync( RolePermissionDBVM model)
        {
            RolePermission rolePermission = await _dbContext.RolePermission.FirstOrDefaultAsync(p => p.RoleId == model.RoleId && p.PermissionId == model.PermissionId);

            if ( rolePermission == null) 
            {
                return null;
            }

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
                await _errorLogFilter.OnException(ex);
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
                rolePermission.IsDeleted = true;

                _dbContext.Entry(rolePermission).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    await _errorLogFilter.OnException(ex);
                }
            }
            return false;
        }

    }
}
