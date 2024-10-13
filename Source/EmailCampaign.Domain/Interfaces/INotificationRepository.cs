using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllNotificationAsync();
        Task<List<Notification>> GetNotificationForIconAsync();
        Task<int> GetUnreadNotificationCountAsync();
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<Notification> UpdateNotificationAsync(Guid id);
        Task<bool> DeleteNotificationAsync(Guid id);

    }
}
