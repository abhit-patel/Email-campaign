using EmailCampaign.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsAddEdit { get; set; }
        public bool IsView { get; set; }
        public bool IsDelete { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }

    }
}
