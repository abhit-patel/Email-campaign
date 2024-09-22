using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> validateUser(string email, string password);

        Task RegisterUser(string username, string password);
    }
}
