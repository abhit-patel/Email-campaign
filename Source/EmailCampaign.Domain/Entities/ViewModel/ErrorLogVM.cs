using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class ErrorLogVM
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public string Stacktrace { get; set; }
        public string InnerException { get; set; }
    }
}
