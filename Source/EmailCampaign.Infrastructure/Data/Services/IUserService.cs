using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Services
{
    public interface IUserService
    {
        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
        Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId );
    }
}
