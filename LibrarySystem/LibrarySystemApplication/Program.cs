using LibrarySystemApplication.Data;
using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Books;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibrarySystemAppContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddDefaultIdentity<Member>(options =>
    options.SignIn.RequireConfirmedAccount = false) // easier for testing
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LibrarySystemAppContext>();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.Run();
