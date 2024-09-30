using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    public class AuthService : IAuthRepository
    {
        private readonly ILoginRepository _loginRepository;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
            _passwordHasher = new PasswordHasher();
        }

        public async Task<User> validateUser(string email, string password)
        {
            var user = await _loginRepository.GetUserByEmailAsync(email);

            if (user == null) return null;

            var isVAlidate =  _passwordHasher.VerifyPassword(password, user.HashPassword, user.SaltKey);

            if (isVAlidate)
            {
                return user;
            }
            return null;
        }


        public async Task<HashedPassVM> RegisterUser(string email, string password)
        {
            HashedPassVM items = _passwordHasher.HashPassword(password);
            return items; 
        }
    }
}
