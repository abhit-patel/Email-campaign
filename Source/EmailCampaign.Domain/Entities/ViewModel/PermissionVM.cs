using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class PermissionVM
    {
        public Guid Id { get; set; }
        public string PageName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Slug { get; set; }
    }
}
