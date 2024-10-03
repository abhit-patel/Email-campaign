using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class RoleVM
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public String RoleName { get; set; }
    }
}
