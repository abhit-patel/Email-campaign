using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmailCampaign.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRoleAsync();
        Task<Role> GetRoleByIdAsync(Guid id);
        Task<Role> GetRoleByNameAsync(string name);
        Task<Role> CreateRoleAsync(string roleName);
        Task<Role> UpdateRoleAsync(Guid id, RoleVM model);
        Task<bool> DeleteRoleAsync(Guid ID);
        Task<List<SelectListItem>> GetRolesAsSelectListItemsAsync();
    }
}
