using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Services;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly INotificationRepository _notificationRepository;
        private readonly ErrorLogFilter _errorLogFilter;


        public RoleRepository(ApplicationDbContext dbContext , IUserContextService userContextService, INotificationRepository notificationRepository, ErrorLogFilter errorLogFilter)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
            _errorLogFilter = errorLogFilter;

        }


        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await _dbContext.Role.Where(p => p.IsDeleted == false).ToListAsync() ;
        }

        public async Task<Role> GetRoleByIdAsync(Guid id)
        {
            return await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
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

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            if (newRole != null)
            {
                var notification = new Notification
                {
                    Header = "New role created.",
                    Body = "User " + _userContextService.GetUserName() + " Created new User role-mapping with " + newRole.Name + " (" + newRole.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Role"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return newRole;
        }


        public async Task<Role> UpdateRoleAsync(Guid id, RoleVM model)
        {

            Role role = await _dbContext.Role.FirstOrDefaultAsync(p => p.Id == id);

            if (role == null) { return new Role(); }

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
                await _errorLogFilter.OnException(ex);
            }

            if (role != null)
            {
                var notification = new Notification
                {
                    Header = "Role details updated.",
                    Body = "User " + _userContextService.GetUserName() + " updated role details with " + role.Name + " (" + role.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Role"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return role;
        }

        public async Task<Role> DeleteRoleAsync(Guid ID)
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
                }
                catch (DbUpdateException ex)
                {
                    await _errorLogFilter.OnException(ex);
                }
            }
            else
            {
                return new Role();
            }

            if (role != null)
            {
                var notification = new Notification
                {
                    Header = "Role Deleted by user.",
                    Body = "User " + _userContextService.GetUserName() + " delete role with " + role.Name + " (" + role.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Role"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return role;
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
