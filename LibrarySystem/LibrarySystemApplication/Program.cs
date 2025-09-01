using LibrarySystemApplication.Data;
using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibrarySystemAppContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));


builder.Services.AddIdentity<Member, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<LibrarySystemAppContext>()
.AddDefaultTokenProviders();



builder.Services.AddScoped<IBookService,  BookService>();
builder.Services.AddScoped<ILibraryServices, LibraryServices>();

builder.Services.AddRazorPages(); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibrarySystemAppContext>();

    if (!context.Books.Any())
    {
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "Books", "Books.json");
        var jsonData = File.ReadAllText(jsonPath);
        var books = JsonSerializer.Deserialize<List<Book>>(jsonData);

        context.Books.AddRange(books);
        context.SaveChanges();
    }
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Librarian", "Member" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Member>>();

    string email = "admin@admin.com";
    string password = "Test1234";

    if (await userManager.FindByEmailAsync(email) == null)
    {

        var member = new Member();
        member.Email = email;
        member.UserName = email;
        member.EmailConfirmed = true;


        await userManager.CreateAsync(member, password);
        await userManager.AddToRoleAsync(member, "Admin");



    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();





app.Run();
