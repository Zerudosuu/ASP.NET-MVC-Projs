using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Implementations;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Implementations;
using LibrarySystemServer.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<LibrarySystemContext>(option =>
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found."
            )
    )
);

builder.Services.AddIdentity<Member, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<LibrarySystemContext>() // <— connects Identity to EF Core
.AddDefaultTokenProviders();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();


var app = builder.Build();

//seed role
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = Enum.GetNames(typeof(MemberRole));

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

//seedng Librarian user
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Member>>();

    string librarianRole = MemberRole.Librarian.ToString();
    string librarianName = "MainLibrarian";
    string librarianEmail = "librarian@librarian.com";
    string librarianPassword = "Test1234";

    var librarianUser = await userManager.FindByNameAsync(librarianName);
    if (librarianUser == null)
    {
        // 3️⃣ Create librarian with full mock details
        var newLibrarian = new Member
        {
            UserName = "main.librarian",
            Email = librarianEmail,
            FirstName = "Alexandria",
            LastName = "Rivera",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1992, 3, 15),
            Address = "123 Library Street, Booktown, Manila, Philippines",
            ProfilePictureUrl = "https://cdn-icons-png.flaticon.com/512/194/194938.png",
            Role = MemberRole.Librarian,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await userManager.CreateAsync(librarianUser, librarianPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(librarianUser, librarianRole);
            Console.WriteLine("✅ Librarian user created successfully with mock profile data!");
        }
        else
        {
            Console.WriteLine("⚠️ Librarian creation failed:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($" - {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("ℹ️ Librarian user already exists.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
