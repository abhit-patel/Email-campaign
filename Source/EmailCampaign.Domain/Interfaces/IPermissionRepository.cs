using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace EmailCampaign.Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllPermissionAsync();
        Task<Permission> GetPermissionAsync(Guid id);
        Task<Permission> CreatePermissionAsync(PermissionVM model);
        Task<Permission> UpdatePermissionAsync(Guid id, PermissionVM model);
        Task<Permission> DeletePermissionAsync(Guid ID);
        Task<List<SelectListItem>> GetPermissionAsSelectListItemsAsync();
    }
}
