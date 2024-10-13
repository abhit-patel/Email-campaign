using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class ContactGroupVM
    {
        public Guid GroupID { get; set; }
        public List<ContactSelection> Contacts { get; set; }
    }

    public class ContactSelection
    {
        public Guid ContactId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public bool IsSelected { get; set; }
    }
}

