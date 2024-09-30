using EmailCampaign.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role {  get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }

        public DbSet<Contact> Contact { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<ContactGroup> ContactGroup { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<User>().HasData(
        //        new User
        //        {
        //            ID = new Guid(),
        //            FirstName = "GD",
        //            LastName = "Admin",
        //            BirthDate = new DateTime(2000, 1, 1),
        //            Email = "admin@gd.com",
        //            Password = "Admin123",
        //            SaltKey = "randomSaltKey123",
        //            HashPassword = "HashedAdminWithSalt123",
        //            ProfilePicture = "profile1.jpg",
        //            RoleId = 1,
        //            IsActive = true,
        //            IsSuperAdmin = false,
        //            ResetpasswordCode = null,
        //            CreatedOn = DateTime.Now,
        //            CreatedBy = 1,
        //            UpdatedOn = DateTime.MinValue,
        //            UpdatedBy = 1,
        //            IsDeleted = false
        //        });

        //}


    }
}
