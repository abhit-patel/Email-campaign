using EmailCampaign.Domain.Entities.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class ContactGroup : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid GroupID { get; set; }
        public Guid ContactId { get; set; }
        public bool IsSubscribed { get; set; }

        public Group Group { get; set; }
        public Contact Contact { get; set; }
    }
}
