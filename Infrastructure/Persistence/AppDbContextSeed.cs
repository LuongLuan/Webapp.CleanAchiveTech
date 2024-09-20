using Domain.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Persistence
{
    public static class AppDbContextSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = new Guid("5E8EFBB5-E59E-4C41-82A7-38AFC8F4EF4E"),
                    UserName = "admin",
                    NormalizedUserName = "admin",
                    Email = "local@admin.com",
                    NormalizedEmail = "local@admin.com",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd1234$"),
                    SecurityStamp = string.Empty,
                    PhoneNumber = "0909900999",
                });
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>()
                {
                    Id = new Guid("0A88D574-73E3-4306-9508-FDB295089D4F"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole<Guid>()
                {
                    Id = new Guid("FDA7EFD3-CE82-421C-9489-7786EB29B87D"),
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                }
                );
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>()
                {
                    RoleId = new Guid("0A88D574-73E3-4306-9508-FDB295089D4F"),
                    UserId = new Guid("5E8EFBB5-E59E-4C41-82A7-38AFC8F4EF4E")
                });
        }
    }
}
