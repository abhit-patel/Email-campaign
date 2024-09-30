using EmailCampaign.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SaltKey { get; set; }
        public string HashPassword { get; set; }
        public string ProfilePicture { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string ResetpasswordCode { get; set; }

        public Role Role { get; set; }

    }
}
