using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class ContactVM
    {
        public Guid Id { get; set; }
        public String FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
    }
}
