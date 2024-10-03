using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class RolePermissionVM
    {
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public List<PermissionListVM> PermissionList { get; set; }
    }


    public class PermissionListVM
    {
        public Guid PermissionId { get; set; }
        public bool IsView { get; set; }
        public bool IsAddEdit { get; set;}
        public bool IsDelete { get; set;}
    }


    public class RolePermissionDBVM
    {
        public Guid ID { get; set; }
        public Guid RoleId { get; set;}
        public Guid PermissionId { get; set; }
        public bool IsView { get; set; }
        public bool IsAddEdit { get; set; }
        public bool IsDelete { get; set; }
    }
}
