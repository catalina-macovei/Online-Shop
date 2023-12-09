using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;

namespace OnlineShop.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService <DbContextOptions<ApplicationDbContext>>()))
            {
                
            if (context.Roles.Any())
                {
                    return; // baza de date contine deja roluri
                }

                // CREAREA ROLURILOR IN BD
              
                context.Roles.AddRange(

                new IdentityRole
                {
                    Id = "cc184db3-076d-416b-b3d3-0d5a8ea1b40f", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                

                new IdentityRole
                {
                    Id = "80a76bbb-5ca5-4a65-8af2-6adf2d2639ad", Name = "Collaborator", NormalizedName = "Collaborator".ToUpper() },
                

                new IdentityRole
                {
                    Id = "a22ae292-ce02-46e1-950c-f73523cbc8e1", Name = "User", NormalizedName = "User".ToUpper() }
                
                );
                
                var hasher = new PasswordHasher<ApplicationUser>();

                // CREAREA USERILOR IN BD
                context.Users.AddRange(
                new ApplicationUser

                {

                    Id = "aff3bcce-5573-44a4-aede-a887e7ebac47",
                    // primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },

                new ApplicationUser
                {

                Id = "2222f049-d9e1-4642-9e93-163ab99ee6ba",
                // primary key
                UserName = "collaborator@test.com",
                EmailConfirmed = true,
                NormalizedEmail = "COLLABORATOR@TEST.COM",
                Email = "collaborator@test.com",
                NormalizedUserName = "COLLABORATOR@TEST.COM",
                PasswordHash = hasher.HashPassword(null, "Collaborator1!")
                },

                new ApplicationUser
                {

                    Id = "d26b10dd-ba36-4365-b3a4-b1ed16939fba",
                    // primary key
                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "User1!")
                });

                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange( 
                    new IdentityUserRole<string>
                {

                    RoleId = "cc184db3-076d-416b-b3d3-0d5a8ea1b40f",
                    UserId = "aff3bcce-5573-44a4-aede-a887e7ebac47"
                    },

                new IdentityUserRole<string>

                {
                    RoleId = "80a76bbb-5ca5-4a65-8af2-6adf2d2639ad",
                    UserId = "2222f049-d9e1-4642-9e93-163ab99ee6ba"
                },

                new IdentityUserRole<string>

                {

                    RoleId = "a22ae292-ce02-46e1-950c-f73523cbc8e1",
                    UserId = "d26b10dd-ba36-4365-b3a4-b1ed16939fba"
                });

                context.SaveChanges();
            }
        }
    }
}
