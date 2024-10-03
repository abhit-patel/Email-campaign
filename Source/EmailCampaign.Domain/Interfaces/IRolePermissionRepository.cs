using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<List<RolePermission>> GetAllAsync();
        Task<RolePermission> GetByIdAsync(Guid id);
        Task<RolePermissionVM> GetAllByIdAsync(Guid id);
        Task<RolePermission> CreateAsync(RolePermissionDBVM model);
        Task<RolePermission> UpdateAsync( RolePermissionDBVM model);
        Task<bool> DeleteAsync(Guid ID);
    }
}
