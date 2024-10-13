using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class ActivityLogVM
    {
        public string Type { get; set; }
        public string EntityType { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public string Slug { get; set; }

    }
}
