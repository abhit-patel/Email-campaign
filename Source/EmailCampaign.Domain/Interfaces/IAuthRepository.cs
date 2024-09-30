using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> validateUser(string email, string password);

        Task<HashedPassVM> RegisterUser(string email, string password);
    }
}
