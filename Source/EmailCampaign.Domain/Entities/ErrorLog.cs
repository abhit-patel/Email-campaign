using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class ErrorLog
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public string Stacktrace { get; set; }
        public string InnerException { get; set; }
        public string PageURL { get; set; }
        public string Device { get; set; }
        public string IPAddress { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
