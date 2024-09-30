using AutoMapper;
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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public UserRepository(ApplicationDbContext dbContext, IAuthRepository authRepository, IMapper mapper, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _authRepository = authRepository;
            _mapper = mapper;
            _userContextService = userContextService;

        }

        public async Task<List<User>> GetAllUserAsync()
        {
            return await _dbContext.User.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid Userid)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.ID == Userid);
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
                IsSuperAdmin = model.IsSuperAdmin,
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
            catch
            {
                throw;
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
                user.Password = model.Password;
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
                throw;
            }
            return user;
        }



        public async Task<bool> DeleteUserAsync(Guid userID)
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
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
            return false;
        }


        public async Task<User> ActiveToggleAsync(Guid userID)
        {
            User user = await _dbContext.User.FirstOrDefaultAsync(p => p.ID == userID);

            if(user != null)
            {
                if (user.IsActive)
                {
                    user.IsActive = false;
                }
                else
                {
                    user.IsActive=true;
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
                throw;
            }

            return user;

        }
    }
}
