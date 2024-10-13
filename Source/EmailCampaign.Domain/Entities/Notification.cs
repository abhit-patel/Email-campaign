using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string RedirectUrl { get; set; }
        public Guid PerformOperationBy { get; set; }
        public Guid PerformOperationFor { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
    }
}
