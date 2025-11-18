using LibrarySystemServer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibrarySystemServer.Services.Seeder
{
    public class SeedHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Member>>();

            var roles = new[] { "Librarian", "Member" };
            string librarianName = "MainLibrarian";

            bool rolesExist = roles.All(role => roleManager.RoleExistsAsync(role).Result);
            bool librarianExists = await userManager.FindByNameAsync(librarianName) != null;

            if (rolesExist && librarianExists)
            {
                Console.WriteLine("⏭ Seeding skipped — roles and librarian already exist.");
                return;
            }

            Console.WriteLine("🔄 Seeding Missing Identity Data...");

            // Create missing roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"   ✔ Role created: {role}");
                }
            }

            // Create Librarian only if missing
            if (!librarianExists)
            {
                var newLibrarian = new Member
                {
                    UserName = librarianName,
                    Email = "librarian@librarian.com",
                    FirstName = "Alexandria",
                    LastName = "Rivera",
                    Gender = Gender.Female,
                    DateOfBirth = new DateTime(1992, 3, 15),
                    Address = "123 Library Street, Book Town, Manila, Philippines",
                    ProfilePictureUrl = "https://cdn-icons-png.flaticon.com/512/194/194938.png",
                    Role = MemberRole.Librarian,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                const string librarianPassword = "Test1234";
                var result = await userManager.CreateAsync(newLibrarian, librarianPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newLibrarian, "Librarian");
                    Console.WriteLine("✅ Librarian user created successfully with mock profile data!");
                }
                else
                {
                    Console.WriteLine("⚠️ Librarian creation failed:");
                    foreach (var error in result.Errors)
                        Console.WriteLine($" - {error.Description}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
