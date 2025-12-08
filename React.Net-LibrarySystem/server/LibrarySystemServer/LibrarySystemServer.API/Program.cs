using System.Text;
using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Implementations;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Auth;
using LibrarySystemServer.Services.Implementations;
using LibrarySystemServer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LibrarySystemServer.CompiledModels;
using LibrarySystemServer.Services.GoogleBooks;
using LibrarySystemServer.Services.Seeder;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddDbContextPool<LibrarySystemContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
        sql => sql.Equals(LibrarySystemContextModel.Instance)
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

builder.Services.AddHostedService<SeedHostedService>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true, 
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();


builder.Services.AddHttpClient<IGoogleBooksClient, GoogleBooksClient>(client =>
    {
        client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
    })
    .AddResilienceHandler("google-books", pipeline =>
    {
        // Automatic retry
        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2),
            UseJitter = true
        });

        // Timeout per request
        pipeline.AddTimeout(TimeSpan.FromSeconds(10));

        // Circuit breaker for repeated failures
        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.1,
            MinimumThroughput = 10,
            SamplingDuration = TimeSpan.FromSeconds(30),
            BreakDuration = TimeSpan.FromSeconds(15)
        });
    });

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<ILibrarianRepository, LibrarianRepository> ();
builder.Services.AddScoped<ILibrarianService, LibrarianService>();

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
