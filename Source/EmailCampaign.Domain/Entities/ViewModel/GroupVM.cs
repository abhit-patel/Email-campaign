using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class GroupVM
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
