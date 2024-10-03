using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class PermissionVM
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Page Name is required")]
        public string PageName { get; set; }
        [Required(ErrorMessage = "Action Name is required")]
        public string ActionName { get; set; }
        [Required(ErrorMessage = "Controller Name is required")]
        public string ControllerName { get; set; }
        [Required(ErrorMessage = "Slug is required")]
        public string Slug { get; set; }
    }
}
