using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Name is Required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required.")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
