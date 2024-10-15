using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Services;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly PasswordHasher _passwordHasher;
        private readonly ErrorLogFilter _errorLogFilter;
        private readonly INotificationRepository _notificationRepository;


        public UserRepository(ApplicationDbContext dbContext, IAuthRepository authRepository, IMapper mapper, IUserContextService userContextService, ErrorLogFilter errorLogFilter, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _authRepository = authRepository;
            _mapper = mapper;
            _userContextService = userContextService;
            _errorLogFilter = errorLogFilter;
            _passwordHasher = new PasswordHasher();
            _notificationRepository = notificationRepository;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            return await _dbContext.User.Where(p => p.IsDeleted == false && p.IsSuperAdmin == false).ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid Userid)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.ID == Userid && p.IsDeleted == false);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<User> GetItemByRoleIDAsync(Guid roleId)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.RoleId == roleId);
        }

        public async Task<bool> CheckRegisteredEmailAsync(string email)
        {
            return await _dbContext.User.AnyAsync(p => p.IsDeleted == false && p.Email == email);
        }


        public async Task<User> CreateUserAsync(UserRegisterVM model)
        {
            HashedPassVM items = await _authRepository.RegisterUser(model.Email, model.Password);

            //User newUser = _mapper.Map<User>(items);

            User newUser = new User()
            {
                ID = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                BirthDate = model.Birthdate,
                Password = model.Password,
                SaltKey = items.saltKey,
                HashPassword = items.hashedPassword,
                ProfilePicture = "",
                RoleId = model.RoleID,
                IsActive = model.IsActive,
                IsSuperAdmin = false,
                ResetpasswordCode = "",
                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false
            };

            await _dbContext.User.AddAsync(newUser);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            if (newUser != null)
            {
                var notification = new Notification
                {
                    Header = "User created",
                    Body = "User created with" + newUser.FirstName + newUser.LastName + " name by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = newUser.CreatedBy,
                    PerformOperationFor = newUser.ID,
                    RedirectUrl = "/Notification"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return newUser;
        }


        public async Task<User> UpdateUserAsync(Guid id, UserRegisterVM model)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == id);


            if (user != null)
            {

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.BirthDate = model.Birthdate;
                user.IsActive = model.IsActive;
                user.IsSuperAdmin = model.IsSuperAdmin;
                user.RoleId = model.RoleID;

                user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                user.UpdatedOn = DateTime.UtcNow;

            }

            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Error updating user with ID {id}", id);
                await _errorLogFilter.OnException(ex);
                throw;
            }

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User Details updated",
                    Body = "Your details updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/Notification"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }


            return user;
        }



        public async Task<User> DeleteUserAsync(Guid userID)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == userID);

            if (user != null)
            {
                user.UpdatedOn = DateTime.UtcNow;
                user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
                user.IsActive = false;
                user.IsDeleted = true;

                _dbContext.Entry(user).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return user;
                }
                catch (DbUpdateException ex)
                {
                    await _errorLogFilter.OnException(ex);
                    throw;
                }
            }
            return new User();
        }


        public async Task<User> SavePasswordResetTokenAsync(string email, string token)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email);

            if (user != null)
            {
                user.ResetpasswordCode = token;

                _dbContext.Entry(user).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return user;
                }
                catch (DbUpdateException ex)
                {
                    await _errorLogFilter.OnException(ex);
                    throw;
                }
            }

            return new User();

        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string email, string token)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email && p.ResetpasswordCode == token);

            if (user != null)
            {
                return true;
            }

            return false;
        }


        public async Task<User> ResetPasswordAsync(string email, string password)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email);

            if (user != null)
            {
                return new User();
            }
            HashedPassVM items = _passwordHasher.HashPassword(password);

            user.Password = password;
            user.HashPassword = items.hashedPassword;
            user.SaltKey = items.saltKey;


            user.UpdatedBy = user.ID;
            user.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "Reset password by mail",
                    Body = "User " + user.FirstName + " " + user.LastName + " updated account's password by " + user.Email + ".",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/Notification"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;

        }

        public async Task<User> ActiveToggleAsync(string email)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email);

            if (user != null)
            {
                if (user.IsActive)
                {
                    user.IsActive = false;
                }
                else
                {
                    user.IsActive = true;
                }
            }

            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(user).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
                throw;
            }

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User IsActive toggle event occur.",
                    Body = "User IsActive status is now " + user.IsActive + "." + " and it's updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/User"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;

        }

        public async Task<User> GetUserInfoForProfile()
        {
            Guid UserId = Guid.Parse(_userContextService.GetUserId());

            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == UserId && p.IsDeleted == false);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User> UpdateProfilePic(IFormFile profilePicture)
        {
            Guid UserId = Guid.Parse(_userContextService.GetUserId());
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProfilePics", UserId.ToString() + ".jpg");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == UserId);

            user.ProfilePicture = "/ProfilePics/" + UserId.ToString() + ".jpg";
            user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            user.UpdatedOn = DateTime.UtcNow;


            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Error updating user with ID {id}", id);
                await _errorLogFilter.OnException(ex);
                throw;
            }

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User update profile picture.",
                    Body = "User " + user.FirstName + " " + user.LastName + " updated it's profile picture. ",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/User/ViewProfile"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;
        }

        public async Task<User> ChangePasswordAsync(string password)
        {
            Guid userId = Guid.Parse(_userContextService.GetUserId());

            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == userId);

            if (user != null)
            {
                return new User();
            }

            //for get hashed new password
            HashedPassVM items = _passwordHasher.HashPassword(password);

            user.Password = password;
            user.HashPassword = items.hashedPassword;
            user.SaltKey = items.saltKey;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = userId;

            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
                throw;
            }

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "Reset password from profile",
                    Body = "User " + user.FirstName + " " + user.LastName + " updated account's password from profile tab.",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/Notification"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;
        }

        public async Task<User> UpdateProfileAsync(ProfileVM model)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.Email == model.Email);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.BirthDate = model.Birthdate;
            user.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            user.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
                throw;
            }

            if (user != null)
            {
                var notification = new Notification
                {
                    Header = "User details updated by self.",
                    Body = "User " + user.FirstName + " " + user.LastName + " updated account's details with " + user.Email + " account.",
                    PerformOperationBy = user.CreatedBy,
                    PerformOperationFor = user.ID,
                    RedirectUrl = "/User/ViewProfile"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return user;
        }
    }
}

