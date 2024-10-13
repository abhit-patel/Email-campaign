using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class ProfileVM
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Birthdate is required")]
        public DateOnly Birthdate { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
    }
}
