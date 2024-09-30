using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class HashedPassVM
    {
        public string hashedPassword {  get; set; }
        public string saltKey { get; set; }
    }
}
