using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories
{
    internal class LoginRepository : ILoginRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public LoginRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.User.FirstOrDefaultAsync(p => p.Email == email);
        }
    }
}
