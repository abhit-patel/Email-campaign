using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        Task<User> GetItemByRoleIDAsync(Guid roleId);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CheckRegisteredEmailAsync(string email);
        Task<User> CreateUserAsync(UserRegisterVM model);
        Task<User> UpdateUserAsync(Guid id, UserRegisterVM model);
        Task<User> DeleteUserAsync(Guid userID);
        Task<User> SavePasswordResetTokenAsync(string email, string token);
        Task<bool> ValidatePasswordResetTokenAsync(string email, string token);
        Task<User> ResetPasswordAsync(string email, string password);
        Task<User> ActiveToggleAsync(string email);
        Task<User> GetUserInfoForProfile();
        Task<User> UpdateProfilePic(IFormFile profilePicture);
        Task<User> ChangePasswordAsync(string password);
        Task<User> UpdateProfileAsync(ProfileVM model);
    }
}
