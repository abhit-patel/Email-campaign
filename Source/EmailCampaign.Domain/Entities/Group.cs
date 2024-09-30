using EmailCampaign.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class Group : BaseEntity
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
