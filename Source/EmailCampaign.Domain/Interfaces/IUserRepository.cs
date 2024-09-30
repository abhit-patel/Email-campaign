using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserAsync(Guid id);
        Task<User> CreateUserAsync(UserRegisterVM model);
        Task<User> UpdateUserAsync(Guid id, UserRegisterVM model);
        Task<bool> DeleteUserAsync(Guid userID);
        Task<User> ActiveToggleAsync(Guid userID);
    }
}
