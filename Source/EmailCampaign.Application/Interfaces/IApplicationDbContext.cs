using EmailCampaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> User { get; set; }
        DbSet<Role> Role { get; set; }
        DbSet<RolePermission> RolePermission { get; set; }
        DbSet<Permission> Permission {  get; set; }
        DbSet<Contact> Contact {  get; set; }
        EntityEntry Entry(object entity);

        Task<int> SaveChangesAsync();
    }
}
