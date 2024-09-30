using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Entities.ViewModel
{
    public class RolePermissionVM
    {
        public Guid RoleId { get; set; }
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
        public Guid RoleId { get; set;}
        public Guid PermissionId { get; set; }
        public bool IsView { get; set; }
        public bool IsAddEdit { get; set; }
        public bool IsDelete { get; set; }
    }
}
