using EmailCampaign.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public Guid Id { get; set; }
        public string PageName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Slug { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }

    }
}
