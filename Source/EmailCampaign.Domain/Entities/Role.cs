﻿using EmailCampaign.Domain.Entities.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
        public ICollection<User> Users { get; set; }

    }
}
