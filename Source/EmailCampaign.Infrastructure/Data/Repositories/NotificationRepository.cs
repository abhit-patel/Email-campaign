using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Log;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IUserContextService _userContextService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ErrorLogFilter _errorLogFilter;

        public NotificationRepository(IUserContextService userContextService, ApplicationDbContext dbContext, ErrorLogFilter errorLogFilter)

        {
            _userContextService = userContextService;
            _dbContext = dbContext;
            _errorLogFilter = errorLogFilter;
        }

        public async Task<List<Notification>> GetAllNotificationAsync()
        {
            Guid userId = Guid.Parse(_userContextService.GetUserId());
            Role role = await _dbContext.User.Where(p => p.ID == userId).Select(p => p.Role).SingleOrDefaultAsync();

            if (role == null)
            {
                return new List<Notification>();
            }

            var permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.ControllerName.ToString().ToUpper() == "NOTIFICATION");
            var rolePermission = await _dbContext.RolePermission.Where(p => p.RoleId == role.Id && p.Permission.ControllerName.ToString().ToUpper() == "NOTIFICATION" && p.IsView == true).FirstOrDefaultAsync();

            if (rolePermission != null)
            {
                return await _dbContext.notification.Where(p => p.IsDeleted == false && p.PerformOperationBy != userId).ToListAsync();
            }

            return await _dbContext.notification.Where(p => p.PerformOperationFor == userId && p.IsDeleted == false && p.PerformOperationBy != userId).ToListAsync();
        }


        public async Task<List<Notification>> GetNotificationForIconAsync()
        {
            Guid userId = Guid.Parse(_userContextService.GetUserId());
            Role role = await _dbContext.User.Where(p => p.ID == userId).Select(p => p.Role).SingleOrDefaultAsync();

            if (role == null)
            {
                return new List<Notification>();
            }

            var permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.ControllerName.ToString().ToUpper() == "NOTIFICATION");
            var rolePermission = await _dbContext.RolePermission.Where(p => p.RoleId == role.Id && p.Permission.ControllerName.ToString().ToUpper() == "NOTIFICATION" && p.IsView == true).FirstOrDefaultAsync();

            if (rolePermission != null)
            {
                return await _dbContext.notification.Where(p => p.IsDeleted == false && p.IsRead == false && p.PerformOperationBy != userId).Take(3).ToListAsync();
            }

            return await _dbContext.notification.Where(p => p.PerformOperationFor == userId && p.IsDeleted == false && p.IsRead == false && p.PerformOperationBy != userId).Take(3).ToListAsync();
        }


        public async Task<int> GetUnreadNotificationCountAsync()
        {
            Guid userId = Guid.Parse(_userContextService.GetUserId());
            Role role = await _dbContext.User.Where(p => p.ID == userId).Select(p => p.Role).SingleOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }

            var permission = await _dbContext.Permission.FirstOrDefaultAsync(p => p.ControllerName.ToString().ToUpper() == "NOTIFICATION");
            var rolePermission = await _dbContext.RolePermission.Where(p => p.RoleId == role.Id && p.Permission.ControllerName.ToString().ToUpper() == "NOTIFICATION" && p.IsView == true).FirstOrDefaultAsync();

            if (rolePermission != null)
            {
                return await _dbContext.notification.Where(p => p.IsDeleted == false && p.IsRead == false && p.PerformOperationBy != userId).CountAsync();
            }

            return await _dbContext.notification.Where(p => p.PerformOperationFor == userId && p.IsDeleted == false && p.IsRead == false && p.PerformOperationBy != userId).CountAsync();
        }


        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;
            notification.IsDeleted = false;

            await _dbContext.notification.AddAsync(notification);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
                throw;
            }

            return notification;
        }


        /// <summary>
        /// Status changed when notification will be read by user.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task<Notification> UpdateNotificationAsync(Guid id)
        {
            Notification item = await _dbContext.notification.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (item == null)
            {
                return new Notification();
            }

            item.IsRead = true;

            _dbContext.Entry(item).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
                throw;
            }

            return item;
        }

        public async Task<bool> DeleteNotificationAsync(Guid id)
        {
            Notification item = await _dbContext.notification.FirstOrDefaultAsync(p => p.Id == id && p.IsRead == true && p.IsDeleted == false);

            if (item == null)
            {
                return false;
            }

            item.IsDeleted = true;

            _dbContext.Entry(item).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
                throw;
            }

            return true;
        }
    }
}
