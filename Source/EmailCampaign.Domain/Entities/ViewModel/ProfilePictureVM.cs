using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class ProfilePictureVM
    {
        [Required(ErrorMessage ="Profile picture must be selected.")]
        public IFormFile ProfilePicture { get; set; }

    }
}
