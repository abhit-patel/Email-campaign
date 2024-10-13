using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string EntityType { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public string Slug { get; set; }
        public string Device { get; set; }
        public string IPAddress { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
