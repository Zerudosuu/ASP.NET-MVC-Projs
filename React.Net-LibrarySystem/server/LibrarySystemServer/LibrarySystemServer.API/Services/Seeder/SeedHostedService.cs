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
            string firstMember = "FirstMemmber";

            // Check roles existence asynchronously (avoid blocking .Result)
            bool rolesExist = true;
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    rolesExist = false;
                    break;
                }
            }

            bool librarianExists = await userManager.FindByNameAsync(librarianName) != null;
            bool firstMemberExists = await userManager.FindByNameAsync(firstMember) != null;

            // Only skip seeding if roles and BOTH accounts already exist. If one of the accounts
            // is missing, continue so we can create it.
            if (rolesExist && librarianExists && firstMemberExists)
            {
                Console.WriteLine("⏭ Seeding skipped — roles and both users already exist.");
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

            if (!firstMemberExists)
            {
                var newMember = new Member
                {
                    UserName = firstMember,
                    Email = "Member@member.com",
                    FirstName = "Ronald",
                    LastName = "Salvador",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1992, 3, 15),
                    Address = "151-4 Library Street, Book Town, Manila, Philippines",
                    ProfilePictureUrl = "https://cdn-icons-png.flaticon.com/512/194/194938.png",
                    Role = MemberRole.Member,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };
                
                const string memberPassword = "Test1234";
                var result = await userManager.CreateAsync(newMember, memberPassword);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newMember, "Member");
                    Console.WriteLine("✅ First member user created successfully with mock profile data!");
                }
                else
                {
                    Console.WriteLine("⚠️ First member creation failed:");
                    foreach (var error in result.Errors)
                        Console.WriteLine($" - {error.Description}");
                }   

            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
