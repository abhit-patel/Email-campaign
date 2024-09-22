using EmailCampaign.Application.Services;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Services
{
    public class AuthService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
            _passwordHasher = new PasswordHasher();
        }

        public async Task<bool> validateUser(string email, string password)
        {
            var user = await _loginRepository.GetUserByEmailAsync(email);

            if (user == null) return false;

            return _passwordHasher.VerifyPassword(password, user.Password, user.SaltKey);
        }


        public async Task RegisterUser(string username, string password)
        {
            var (hashedPassword, salt) = _passwordHasher.HashPassword(password);
            var user = new User { Email = username, PasswordHash = hashedPassword, SaltKey = salt };
            //await _userRepository.CreateUserAsync(user);
        }
    }
}
